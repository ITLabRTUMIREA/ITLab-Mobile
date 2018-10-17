using Http_Post.Extensions.Responses.Event;
using Http_Post.Res;
using Http_Post.Services;
using Models.PublicAPI.Responses.Equipment;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Http_Post.Pages
{
	public partial class CreateEquipment : ContentPage
	{
        private HttpClient client = HttpClientFactory.HttpClient;
        private Guid? ownerId; // if empty - laboratory, if not empty - has owner
        private UserView newUser;
        private EquipmentTypeView newETV;
        private bool isCreating; // if creating - true, if changing - false
        private Guid? eqId; // if creating - null, if changing - use already existed

        // pass to this ctor if needed to change equipment
        public CreateEquipment (EquipmentViewExtended equipment)
        {
            Init();
            isCreating = false;
            eqId = equipment.Id;

            editEquipType.Text = equipment.EquipmentType.Title;
            Hide(null, null);
            editSerialNumber.Text = equipment.SerialNumber;
            editDescription.Text = equipment.Description;
            lblOwner.Text = equipment.OwnerName;
            newETV = equipment.EquipmentType;
            ownerId = equipment.OwnerId;

            btnConfirm.BackgroundColor = btnChangeOwner.BackgroundColor;
        }

        public CreateEquipment ()
        {
            Init();
            isCreating = true;
        }

        private void Init()
        {
            InitializeComponent();
            UpdateLanguage();
        }

        // if user write smth 'Confirm' btn will glow green to show - you need to save it
        private void DefaultTextChanged(object sender, TextChangedEventArgs e)
        {
            SetGreenColorToConfirm();
        }

        private void SetGreenColorToConfirm()
        {
            btnConfirm.BackgroundColor = Color.FromHex("#004d00");
        }

        private async void editEquipType_TextChanged(object sender, TextChangedEventArgs e)
        {
            Show();
            SetGreenColorToConfirm();
            stack.Children.Clear();
            try
            {
                var lbl = (Editor)sender;
                var response = await client.GetStringAsync($"EquipmentType?match={lbl.Text}&all=true");
                var equipType = JsonConvert.DeserializeObject<ListResponse<EquipmentTypeView>>(response);
                if (equipType.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {equipType.StatusCode}");

                Style styleLbl = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Lbl"] as Style;
                foreach (var eq in equipType.Data)
                {
                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += (s, args) => {
                        Hide(null, null);
                        editEquipType.Text = eq.Title;
                        newETV = eq;
                        editSerialNumber.Focus();
                    };
                    var label = new Label
                    {
                        Style = styleLbl,
                        Text = eq.Title
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
        }

        private async void btnConfirm_Clicked(object sender, EventArgs e)
        {
            btnConfirm.Style = btnChangeOwner.Style;
            try
            {
                if (newETV == null)
                    throw new Exception($"Error: {Resource.EquipmentType} is null");

                if (string.IsNullOrEmpty(editDescription.Text) || string.IsNullOrWhiteSpace(editDescription.Text))
                    throw new Exception($"Error: {Resource.Description} is null");

                if (string.IsNullOrEmpty(editSerialNumber.Text) || string.IsNullOrWhiteSpace(editSerialNumber.Text))
                    throw new Exception($"Error: {Resource.SerialNumber} is null");

                Guid? ownerGuid = newUser == null ? ownerId : newUser.Id;
                EquipmentView equipmentView = new EquipmentView
                {
                    Id = eqId ?? Guid.Empty, 
                    OwnerId = ownerGuid ?? null,
                    EquipmentType = newETV,
                    EquipmentTypeId = newETV.Id,
                    Description = editDescription.Text,
                    SerialNumber = editSerialNumber.Text
                };

                var jsonContent = JsonConvert.SerializeObject(equipmentView);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var result = isCreating ?
                    await client.PostAsync("Equipment/", content) : await client.PutAsync("Equipment/", content);
                var resultContent = await result.Content.ReadAsStringAsync();
                var message = JsonConvert.DeserializeObject<OneObjectResponse<EquipmentView>>(resultContent);
                if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {message.StatusCode}");

                await DisplayAlert("", Resource.ADMIN_Updated, "Ok");

                await Navigation.PushAsync(new OneEquipmentPage(message.Data.Id));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private async void btnCreateEqType_Clicked(object sender, EventArgs e)
            => await CreateEquipmentType(Navigation);
        
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

                var lbl = new Label { Text = Resource.Create,
                    HorizontalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold,
                    Style = styleLbl };
                var entryTitle = new Editor { Text = editEquipType.Text,
                    Placeholder = Resource.EquipmentType,
                    Style = styleLbl, };
                var entryDescription = new Editor { Text = "",
                    Placeholder = Resource.Description,
                    Style = styleLbl, };

                entryTitle.Completed += (s, e) 
                    => entryDescription.Focus();

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
                    editEquipType.Text = message.Data.Title;
                    newETV = message.Data;
                    editSerialNumber.Focus();
                    SetGreenColorToConfirm();
                    // pass result
                    tcs.SetResult(null);
                };

                var btnCancel = new Button
                {
                    Text = Resource.ADMIN_Cancel,
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

        private async void btnChangeOwner_Clicked(object sender, EventArgs e)
            => await ChangeOwner(Navigation);

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

                var lbl = new Label
                {
                    Text = lblOwnerTitle.Text,
                    HorizontalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold,
                    Style = styleLbl
                };
                var edit = new Editor
                {
                    Text = "",
                    Placeholder = "Start typing", // TODO: think what to do with this
                    Style = styleLbl,
                };
                var stackOwner = new StackLayout
                {
                    IsVisible = false,
                    VerticalOptions = LayoutOptions.StartAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Orientation = StackOrientation.Vertical,
                    Style = styleStack,
                };
                // while user types - find owners
                edit.TextChanged += async (sender, e) =>
                {
                    try
                    {
                        stackOwner.IsVisible = true;
                        stackOwner.Children.Clear();
                        var txt = (Editor)sender;
                        var response = await client.GetStringAsync($"user/count/?email={txt.Text}&count=10&firstname={txt.Text}&lastname={txt.Text}");
                        var users = JsonConvert.DeserializeObject<ListResponse<UserView>>(response);
                        if (users.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                            throw new Exception($"Error: {users.StatusCode}");

                        foreach (var u in users.Data)
                        {
                            var tapGestureRecognizer = new TapGestureRecognizer();
                            tapGestureRecognizer.Tapped += (s, args) =>
                            {
                                stackOwner.IsVisible = false;
                                newUser = u;
                            };
                            var label = new Label
                            {
                                Style = styleLbl,
                                Text = u.FirstName + " " + u.LastName + ", " + u.Email
                            };
                            label.GestureRecognizers.Add(tapGestureRecognizer);
                            stackOwner.Children.Add(label);
                            if (stackOwner.Children.Count >= 5)
                                break;
                        };
                    }
                    catch (Exception ex)
                    {
                        stackOwner.Children.Add(new Label
                        {
                            Text = ex.Message,
                            Style = styleLbl,
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
                    if (string.IsNullOrEmpty(edit.Text) || string.IsNullOrWhiteSpace(edit.Text) 
                        || newUser == null)
                    {
                        edit.Focus();
                        return;
                    }

                    lblOwner.Text = newUser.FirstName + " " + newUser.LastName + ", " + newUser.Email;

                    await navigation.PopModalAsync();
                    SetGreenColorToConfirm();
                    // pass result
                    tcs.SetResult(null);
                };

                var btnCancel = new Button
                {
                    Text = Resource.ADMIN_Cancel,
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
                layout.Children.Add(edit);
                layout.Children.Add(stackOwner);
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

        private void UpdateLanguage()
        {
            Title = Resource.TitleCreateEquipment;
            btnCreateEqType.Text = Resource.Create + " " + Resource.EquipmentType;
            btnConfirm.Text = Resource.Save;
            btnChangeOwner.Text = Resource.ChangeOwner;
            lblEquipType.Text = editEquipType.Placeholder = Resource.EquipmentType;
            lblSerialNumber.Text = editSerialNumber.Placeholder = Resource.SerialNumber;
            lblDescription.Text = editDescription.Placeholder = Resource.Description;
            lblOwnerTitle.Text = Resource.Owner;
        }

        private void Show()
        {
            stack.IsVisible = true;
            btnCreateEqType.IsVisible = true;
        }

        private void Hide(object sender, FocusEventArgs e)
        {
            stack.IsVisible = false;
            btnCreateEqType.IsVisible = false;
        }
    }
}