using Http_Post.Res;
using Http_Post.Services;
using Models.PublicAPI.Responses.Equipment;
using Models.PublicAPI.Responses.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Http_Post.Pages
{
	public partial class CreateEquipment : ContentPage
	{
        private HttpClient client = HttpClientFactory.HttpClient;
        private Guid ownerId;
        public CreateEquipment (Guid id)
		{
			InitializeComponent ();

            ownerId = id;
            UpdateLanguage();
		}

        private async void EntryEquipType_TextChanged(object sender, TextChangedEventArgs e)
        {
            Show();
            try
            {
                // TODO: replace with stacklayout
                var lbl = (Entry)sender;
                var response = await client.GetStringAsync($"EquipmentType?match={lbl.Text}");
                var equipType = JsonConvert.DeserializeObject<ListResponse<EquipmentTypeView>>(response);
                if (equipType.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {equipType.StatusCode}");

                listView.ItemsSource = equipType.Data;
            }
            catch (Exception ex)
            {

            }
        }

        private EquipmentTypeView newETV;

        private async void BtnConfirm_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (newETV == null)
                    throw new Exception("Error: equipment type is null");

                if (string.IsNullOrEmpty(Entry_Description.Text) || string.IsNullOrWhiteSpace(Entry_Description.Text))
                    throw new Exception("Error: description is null");

                if (string.IsNullOrEmpty(Entry_SerialNumber.Text) || string.IsNullOrWhiteSpace(Entry_SerialNumber.Text))
                    throw new Exception("Error: serial number is null");

                EquipmentView equipmentView = new EquipmentView
                {
                    OwnerId = ownerId,
                    EquipmentType = newETV,
                    EquipmentTypeId = newETV.Id,
                    Description = Entry_Description.Text,
                    SerialNumber = Entry_SerialNumber.Text
                };

                var jsonContent = JsonConvert.SerializeObject(equipmentView);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var result = await client.PostAsync("Equipment/", content);
                var resultContent = await result.Content.ReadAsStringAsync();
                var message = JsonConvert.DeserializeObject<OneObjectResponse<EquipmentView>>(resultContent);
                if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {message.StatusCode}");

                await DisplayAlert("", Resource.ADMIN_Updated, "Ok");

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private async void BtnCreateEqType_Clicked(object sender, EventArgs e)
        {
            await InputBox(Navigation);
        }
        
        // TODO: replace with plugin
        public Task<string> InputBox(INavigation navigation)
        {
                // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<string>();
            try
            {
                var st = Application.Current.Resources;
                var th = new Classes.ThemeChanger().Theme;
                Style styleLbl = st[th + "_Lbl"] as Style;
                Style styleBtn = st[th + "_Btn"] as Style;
                Style styleStack = st[th + "_Stack"] as Style;

                var lbl = new Label { Text = "Create", HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold, Style = styleLbl };
                var entryTitle = new Entry { Text = "", Placeholder = "Title", Style = styleLbl, PlaceholderColor = lbl.TextColor };
                var entryDesciption = new Entry { Text = "", Placeholder = "Desciption", Style = styleLbl, PlaceholderColor = lbl.TextColor };

                entryTitle.Completed += (s, e) =>
                { entryDesciption.Focus(); };

                var btnOk = new Button
                {
                    Text = "Ok",
                    WidthRequest = 100,
                    Style = styleBtn
                };
                btnOk.Clicked += async (s, e) =>
                {
                    // close page
                    var newEqType = new EquipmentTypeView
                    {
                        Title = entryTitle.Text,
                        Description = entryDesciption.Text,
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
                    Entry_EquipType.Text = message.Data.Title;
                    newETV = message.Data;
                    Entry_SerialNumber.Focus();
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
                    Children = { btnOk, btnCancel },
                    Style = styleStack
                };

                var layout = new StackLayout
                {
                    Padding = new Thickness(0, 40, 0, 0),
                    VerticalOptions = LayoutOptions.StartAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Orientation = StackOrientation.Vertical,
                    Style = styleStack,
                    Children = { lbl, entryTitle, entryDesciption, slButtons },
                };

                // create and show page
                var page = new ContentPage()
                {
                    Style = styleStack
                };
                page.Content = layout;
                navigation.PushModalAsync(page);
                // open keyboard
                entryTitle.Focus();

                // code is waiting her, until result is passed with tcs.SetResult() in btn-Clicked
                // then proc returns the result
                return tcs.Task;
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "Ok");
                return tcs.Task;
            }
        }

        private void UpdateLanguage()
        {
            Title = Resource.Title_Create;
            Btn_CreateEqType.Text = "Create new Type";
            Btn_Confirm.Text = "Confirm"; // TODO: after all - open One Equipment page with this NEW EQUIP
            Lbl_EquipType.Text = Entry_EquipType.Placeholder = "Type";
            Lbl_SerialNumber.Text = Entry_SerialNumber.Placeholder = "Serial Number";
            Lbl_Description.Text = Entry_Description.Placeholder = "Description";
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Entry_EquipType.Text = ((EquipmentTypeView)e.Item).Title;
            newETV = (EquipmentTypeView)e.Item;
            Hide();
            Entry_SerialNumber.Focus();
        }

        private void Show()
        {
            listView.IsVisible = true;
            Btn_CreateEqType.IsVisible = true;
        }

        private void Hide()
        {
            listView.IsVisible = false;
            Btn_CreateEqType.IsVisible = false;
        }

        private void Entry_SerialNumber_Focused(object sender, FocusEventArgs e)
        {
            Hide();
        }

    }
}