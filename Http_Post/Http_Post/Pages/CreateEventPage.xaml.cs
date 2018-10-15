using Http_Post.Res;
using Models.PublicAPI.Responses.Event;
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
	public partial class CreateEventPage : ContentPage
	{
        private HttpClient client = Services.HttpClientFactory.HttpClient;
        private EventTypeView newETV;

        public CreateEventPage ()
		{
			InitializeComponent ();
            UpdateLanguage();
		}

        private void UpdateLanguage()
        {
            Title = Resource.Title_Create;
            lblEventType.Text = "Event Type";
            lblName.Text = "Name";
            lblDescription.Text = Resource.Description;
            lblAddress.Text = "Address";
            btnAddShift.Text = Resource.Shifts;
            btnCreateEvent.Text = Resource.Title_Create;
            ///////////////////////////////////////
            editEventType.Placeholder = lblEventType.Text;
            editName.Placeholder = lblName.Text;
            editDescription.Placeholder = lblDescription.Text;
            editAddress.Placeholder = lblAddress.Text;
        }

        private void editEventType_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TODO: запрос на уже сущесвующие
            // подлглядеть в EquipmentCreate
        }

        private async void btnCreateEventType_Clicked(object sender, EventArgs e)
        {
            await InputBox(Navigation);
        }

        private async void btnCreateEvent_Clicked(object sender, EventArgs e)
        {
            if (!await CheckAllFieldsForNull())
                return;

            // If every thing is Ok - create event
            CreateEvent();
        }

        private async void CreateEvent()
        {

        }

        private async Task<bool> CheckAllFieldsForNull()
        {
            // Event type
            if (string.IsNullOrEmpty(editEventType.Text))
            {
                await DisplayAlert("Error", $"Please add '{lblEventType.Text}'", "Ok");
                editEventType.Focus();
                return false;
            }

            // Event name
            if(string.IsNullOrEmpty(editName.Text))
            {
                await DisplayAlert("Error", $"Please add '{lblName.Text}'", "Ok");
                editName.Focus();
                return false;
            }

            // Event description
            if (string.IsNullOrEmpty(editDescription.Text))
            {
                await DisplayAlert("Error", $"Please add '{lblDescription.Text}'", "Ok");
                editDescription.Focus();
                return false;
            }

            // Event Address
            if (string.IsNullOrEmpty(editAddress.Text))
            {
                await DisplayAlert("Error", $"Please add '{lblAddress.Text}'", "Ok");
                editAddress.Focus();
                return false;
            }

            // Event's shifts
            if (stackShifts.Children.Count <= 0)
            {
                await DisplayAlert("Error", $"Please add '{Resource.Shifts}'", "Ok");
                return false;
            }

            return true;
        }

        public Task<string> InputBox(INavigation navigation)
        { 
            var tcs = new TaskCompletionSource<string>();
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
                var entryTitle = new Editor { Text = editEventType.Text, // if no such title - add it to new event type
                    Placeholder = "Title",
                    Style = styleLbl, };
                var entryDescription = new Editor { Text = "",
                    Placeholder = "Desciption",
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

                    var newEventType = new EventTypeView
                    {
                        Title = entryTitle.Text,
                        Description = entryDescription.Text,
                        // TODO: wait for Maksim PARENT and CHILD's
                    };
                    var jsonContent = JsonConvert.SerializeObject(newEventType);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync("EventType", content);

                    var resultContent = await result.Content.ReadAsStringAsync();
                    var message = JsonConvert.DeserializeObject<OneObjectResponse<EventTypeView>>(resultContent);
                    if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                        throw new Exception($"Error: {message.StatusCode}");

                    await navigation.PopModalAsync();
                    editEventType.Text = message.Data.Title;
                    newETV = message.Data;
                    editName.Focus();
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
                    await navigation.PopModalAsync();
                    tcs.SetResult(null);
                };

                var slButtons = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Center,
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
                    Children = { lbl, entryTitle, entryDescription, slButtons },
                };

                var page = new ContentPage()
                {
                    Style = styleStack
                };
                page.Content = layout;
                navigation.PushModalAsync(page);
                entryTitle.Focus();
                return tcs.Task;
            }
            catch (Exception ex)
            {
                // TODO: think how to show error to user
                return tcs.Task;
            }
        }

        private void Show()
        {
            stackEventType.IsVisible = true;
            btnCreateEventType.IsVisible = true;
        }

        private void Hide()
        {
            stackEventType.IsVisible = false;
            btnCreateEventType.IsVisible = false;
        }

        private void editName_Focused(object sender, FocusEventArgs e)
        {
            Hide();
        }
    }
}