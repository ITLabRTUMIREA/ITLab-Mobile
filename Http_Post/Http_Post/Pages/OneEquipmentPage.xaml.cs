using System;
using Http_Post.Res;
using Http_Post.Services;

using Xamarin.Forms;
using System.Net.Http;
using Http_Post.Extensions.Responses.Event;
using Newtonsoft.Json;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;
using Models.PublicAPI.Responses.Equipment;
using System.Text;
using Models.PublicAPI.Responses.Exceptions;
using System.Threading.Tasks;

namespace Http_Post.Pages
{
	public partial class OneEquipmentPage : ContentPage
	{
        private Guid EquipId { get; }
        private Guid OwnerId;
        private UserView newUser;
        private HttpClient client = HttpClientFactory.HttpClient;
        private EquipmentViewExtended equipment { get; set; }
        private Action updateEquip;
        private EquipmentTypeView newETV;

        public OneEquipmentPage (Guid equipId, Action actionToUpdateEquip)
		{
			InitializeComponent ();

            updateEquip = actionToUpdateEquip;
            EquipId = equipId;
            UpdateLanguage();
            GetEquip();
		}


        private async void GetEquip()
        {
            try
            {
                var response = await client.GetStringAsync($"Equipment/{EquipId}");
                var equip = JsonConvert.DeserializeObject<OneObjectResponse<EquipmentViewExtended>>(response);

                if (equip.Data.OwnerId != null)
                {
                    response = await client.GetStringAsync($"user/{equip.Data.OwnerId}");
                    var user = JsonConvert.DeserializeObject<OneObjectResponse<UserView>>(response);
                    equip.Data.OwnerName = user.Data.FirstName + " " + user.Data.LastName + ", " +
                        user.Data.Email;
                }
                else
                    equip.Data.OwnerName = Resource.ADMIN_Laboratory;

                equipment = equip.Data;
                SetInfo();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private void SetInfo()
        {
            editType.Text = equipment.EquipmentType.Title;
            editNumber.Text = equipment.SerialNumber;
            editDescription.Text = equipment.Description;
            lblOwner.Text = equipment.OwnerName;
        }

        private void UpdateLanguage()
        {
            Title = Resource.Title_Equipment;
            lblType.Text = Resource.Equipment_Type;
            lblNumber.Text = Resource.Equipment_SerialNumber;
            lblDescription.Text = Resource.Description;
            lblOwnerTitle.Text = Resource.Equipment_Owner;
            /////////////////////////////////////////////////////
            btnConfirm.Text = Resource.Equipment_BtnConfirm;
            btnChangeOwner.Text = Resource.Equipment_BtnChangeOwner;
            btnDelete.Text = Resource.Equipment_BtnDelete;
            btnCreateType.Text = "Create equipment type"; // TODO: localization
            ////////////////////////////////////////////////////
            btnDelete.BackgroundColor = Color.FromHex("#ff8080"); // Pretty red
        }

        private async void btnConfirm_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool save = await DisplayAlert("", Resource.ADMIN_Sure, Resource.ADMIN_Yes, Resource.ADMIN_No);
                if (save) {
                    EquipmentView equipmentView = new EquipmentView
                    {
                        Id = equipment.Id, // BY DEFAULT
                        EquipmentTypeId = equipment.EquipmentTypeId, // BY DEFAULT
                        EquipmentType = newETV ?? new EquipmentTypeView
                        {
                            Title = editType.Text,
                            Description = equipment.EquipmentType.Description,
                            Id = equipment.EquipmentTypeId,
                            Childs = equipment.EquipmentType.Childs, // BY DEFAULT
                            Parent = equipment.EquipmentType.Parent // BY DEFAULT
                        },
                        Description = editDescription.Text,
                        SerialNumber = editNumber.Text,
                        OwnerId = newUser == null ? equipment.OwnerId : newUser.Id // if we change owner it won't be null
                    };

                    var jsonContent = JsonConvert.SerializeObject(equipmentView);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var result = await client.PutAsync("Equipment/", content);
                    string resultContent = await result.Content.ReadAsStringAsync();
                    var newEquip = JsonConvert.DeserializeObject<OneObjectResponse<EquipmentView>>(resultContent);

                    if (newEquip.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                        throw new Exception($"Error: {newEquip.StatusCode}");
                    else
                        await DisplayAlert("Message", Resource.ADMIN_Updated, "Ok");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private async void btnChangeOwner_Clicked(object sender, EventArgs e)
        {
            await ChangeOwner(Navigation);
        }

        private async void btnDelete_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool delete = await DisplayAlert("", Resource.ADMIN_Sure, Resource.ADMIN_Yes, Resource.ADMIN_No);
                if (delete)
                {
                    var jsonConent = $"{{\"id\":\"{equipment.Id}\"}}";

                    var request = new HttpRequestMessage(HttpMethod.Delete, "Equipment") {
                        Content = new StringContent(jsonConent, Encoding.UTF8, "application/json")
                    };
                    var result = await client.SendAsync(request);

                    string resultContent = await result.Content.ReadAsStringAsync();
                    var message = JsonConvert.DeserializeObject<ExceptionResponse>(resultContent);
                    if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                        throw new Exception($"Error: {message.StatusCode}");

                    await DisplayAlert("", Resource.ADMIN_Updated, "Ok");
                    updateEquip?.Invoke();
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        public Task<string> ChangeOwner(INavigation navigation)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<string>();
            var layout = new StackLayout
            {
                Padding = new Thickness(0, 40, 0, 0),
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Vertical,
                Style = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Stack"] as Style,
            };
            try
            {
                var th = new Classes.ThemeChanger().Theme;
                Style styleLbl = Application.Current.Resources[th + "_Lbl"] as Style;
                Style styleBtn = Application.Current.Resources[th + "_Btn"] as Style;
                Style styleStack = Application.Current.Resources[th + "_Stack"] as Style;

                var lbl = new Label { Text = "Change",
                    HorizontalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold,
                    Style = styleLbl };
                var edit = new Editor { Text = "",
                    Placeholder = "Enter name here",
                    Style = styleLbl, };
                var stack = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    HorizontalOptions = LayoutOptions.Center,
                    IsVisible = false,
                    Style = styleStack
                }; // stack for adding here names of people

                edit.TextChanged += async (sender, e) =>
                {
                    stack.IsVisible = true;
                    stack.Children.Clear();
                    try
                    {
                        var txt = (Editor)sender;
                        var response = await client.GetStringAsync($"user/count/?email={txt.Text}&count=10&firstname={txt.Text}&lastname={txt.Text}&match=gmail");
                        var users = JsonConvert.DeserializeObject<ListResponse<UserView>>(response);
                        if (users.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                            throw new Exception($"Error: {users.StatusCode}");

                        Style st = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Lbl"] as Style;
                        foreach (var u in users.Data)
                        {
                            var tapGestureRecognizer = new TapGestureRecognizer();
                            tapGestureRecognizer.Tapped += (s, args) =>
                            {
                                stack.IsVisible = false;
                                edit.Text = u.FirstName + " " + u.LastName;
                                newUser = u;
                            };
                            var label = new Label
                            {
                                Style = st,
                                Text = u.FirstName + " " + u.LastName
                            };
                            label.GestureRecognizers.Add(tapGestureRecognizer);
                            stack.Children.Add(label);
                            if (stack.Children.Count >= 5)
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        stack.Children.Add(new Label
                        {
                            Text = ex.Message,
                            Style = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Lbl"] as Style
                        });
                    }
                };

                var btnOk = new Button
                {
                    Text = "Ok",
                    WidthRequest = 100,
                    Style = styleBtn
                };
                btnOk.Clicked += async (s, e) =>
                {
                    if (string.IsNullOrEmpty(edit.Text) || string.IsNullOrWhiteSpace(edit.Text))
                    {
                        edit.Focus();
                        return;
                    }

                    if (newUser == null)
                        tcs.SetResult(null);

                    btnConfirm.BackgroundColor = Color.FromHex("#17cf54");
                    lblOwner.Text = newUser.FirstName + " " + newUser.LastName + ", " +
                        newUser.Email;

                    // change, close
                    await navigation.PopModalAsync();
                    btnConfirm.Focus();
                    // pass result
                    tcs.SetResult(null);
                };

                var btnCancel = new Button
                {
                    Text = "Cancel",
                    WidthRequest = 100,
                    Style = styleBtn
                };
                btnCancel.Clicked += async (s, e) =>
                {
                    // close page
                    await navigation.PopModalAsync();
                    // pass empty result
                    tcs.SetResult(null);
                };

                var slButtons = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Center,
                    Children = { btnOk, btnCancel },
                    Style = styleStack
                };

                layout.Children.Add(lbl);
                layout.Children.Add(stack);
                layout.Children.Add(edit);
                layout.Children.Add(slButtons);

                // create and show page
                var page = new ContentPage()
                {
                    Style = styleStack
                };
                page.Content = layout;
                navigation.PushModalAsync(page);
                // open keyboard
                edit.Focus();
                return tcs.Task;
            }
            catch (Exception ex)
            {
                var itisLabel = layout.Children[layout.Children.Count - 1];
                if (itisLabel.Equals(new Label()))
                {
                    ((Label)layout.Children[layout.Children.Count - 1]).Text = ex.Message;
                }
                else
                {
                    layout.Children.Add(new Label
                    {
                        Text = ex.Message,
                        Style = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Lbl"] as Style,
                        HorizontalOptions = LayoutOptions.Center
                    });
                }
                return tcs.Task; // while Task != "good" -> invoke Task
            }
        }

        private async void btnCreateType_Clicked(object sender, EventArgs e)
        {
            await CreateEquipmentType(Navigation);
        }

        private async void editType_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnCreateType.IsVisible = true;
            stackCreateType.IsVisible = true;
            stackCreateType.Children.Clear();
            try
            {
                var txt = (Editor)sender;
                var response = await client.GetStringAsync($"EquipmentType?match={txt.Text}&all=true");
                var equipType = JsonConvert.DeserializeObject<ListResponse<EquipmentTypeView>>(response);
                if (equipType.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {equipType.StatusCode}");

                Style styleLbl = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Lbl"] as Style;
                foreach (var eq in equipType.Data)
                {
                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += (s, args) => {
                        btnCreateType.IsVisible = false;
                        stackCreateType.IsVisible = false;
                        editType.Text = eq.Title;
                        newETV = eq;
                        editNumber.Focus();
                    };
                    var label = new Label
                    {
                        Style = styleLbl,
                        Text = eq.Title
                    };
                    label.GestureRecognizers.Add(tapGestureRecognizer);
                    stackCreateType.Children.Add(label);
                    if (stackCreateType.Children.Count >= 5)
                        break;
                }

            }
            catch (Exception ex)
            {
                stackCreateType.Children.Add(new Label
                {
                    Text = ex.Message,
                    Style = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Lbl"] as Style
                });
            }
        }

        public Task<string> CreateEquipmentType(INavigation navigation)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<string>();
            var layout = new StackLayout
            {
                Padding = new Thickness(0, 40, 0, 0),
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Vertical,
                Style = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Stack"] as Style,
            };
            try
            {
                var th = new Classes.ThemeChanger().Theme;
                Style styleLbl = Application.Current.Resources[th + "_Lbl"] as Style;
                Style styleBtn = Application.Current.Resources[th + "_Btn"] as Style;
                Style styleStack = Application.Current.Resources[th + "_Stack"] as Style;

                var lbl = new Label { Text = "Create",
                    HorizontalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold,
                    Style = styleLbl };
                var entryTitle = new Editor { Text = editType.Text,
                    Placeholder = "Title", 
                    Style = styleLbl, };
                var entryDescription = new Editor { Text = "",
                    Placeholder = "Desciption",
                    Style = styleLbl, };

                entryTitle.Completed += (s, e) =>
                { entryDescription.Focus(); };

                var btnOk = new Button
                {
                    Text = "Ok",
                    WidthRequest = 100,
                    Style = styleBtn
                };
                btnOk.Clicked += async (s, e) =>
                {
                    if (string.IsNullOrEmpty(entryTitle.Text) || string.IsNullOrWhiteSpace(entryTitle.Text))
                    {
                        entryTitle.Focus();
                        return;
                    }

                    if (string.IsNullOrEmpty(entryDescription.Text) || string.IsNullOrWhiteSpace(entryDescription.Text))
                    {
                        entryDescription.Focus();
                        return;
                    }

                    var newEqType = new EquipmentTypeView
                    {
                        Title = entryTitle.Text,
                        Description = entryDescription.Text,
                        // TODO: wait for Maksim PARENT and CHILD's
                    };
                    var jsonContent = JsonConvert.SerializeObject(newEqType);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync("EquipmentType", content);

                    var resultContent = await result.Content.ReadAsStringAsync();
                    var message = JsonConvert.DeserializeObject<OneObjectResponse<EquipmentTypeView>>(resultContent);
                    if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                        throw new Exception($"Error: {message.StatusCode}");

                    await navigation.PopModalAsync();
                    editType.Text = message.Data.Title;
                    newETV = message.Data;
                    editNumber.Focus();
                    // pass result
                    tcs.SetResult(null);
                };

                var btnCancel = new Button
                {
                    Text = "Cancel",
                    WidthRequest = 100,
                    Style = styleBtn
                };
                btnCancel.Clicked += async (s, e) =>
                {
                    // close page
                    await navigation.PopModalAsync();
                    // pass empty result
                    tcs.SetResult(null);
                };

                var slButtons = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Center,
                    Children = { btnOk, btnCancel },
                    Style = styleStack
                };

                layout.Children.Add(lbl);
                layout.Children.Add(entryTitle);
                layout.Children.Add(entryDescription);
                layout.Children.Add(slButtons);

                // create and show page
                var page = new ContentPage()
                {
                    Style = styleStack
                };
                page.Content = layout;
                navigation.PushModalAsync(page);
                // open keyboard
                entryTitle.Focus();
                return tcs.Task;
            }
            catch (Exception ex)
            {
                var itisLabel = layout.Children[layout.Children.Count - 1];
                if (itisLabel.Equals(new Label()))
                {
                    ((Label)layout.Children[layout.Children.Count - 1]).Text = ex.Message;
                }
                else
                {
                    layout.Children.Add(new Label
                    {
                        Text = ex.Message,
                        Style = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Lbl"] as Style,
                        HorizontalOptions = LayoutOptions.Center
                    });
                }
                return tcs.Task; // while Task != "good" -> invoke Task
            }
        }
    }
}