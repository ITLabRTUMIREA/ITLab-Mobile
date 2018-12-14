using Http_Post.Res;
using Models.PublicAPI.Requests.Events.Event.Create;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateEventPage : ContentPage
	{
        HttpClient client = Services.HttpClientFactory.HttpClient;
        Guid eventTypeId;
        Guid eventId;
        List<ShiftCreateRequest> ShiftCreate = new List<ShiftCreateRequest>();        

        public CreateEventPage ()
		{
            InitializeComponent();
            UpdateLanguage();
        }
        
        // find event type
        async void editEventType_TextChanged(object sender, TextChangedEventArgs e)
        {
            Show();
            stackEventType.Children.Clear();
            try
            {
                var lbl = (Editor)sender;
                var response = await client.GetStringAsync($"EventType?all=true&match={lbl.Text}");
                var equipType = JsonConvert.DeserializeObject<ListResponse<EventTypeView>>(response);
                if (equipType.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {equipType.StatusCode}");

                foreach (var eq in equipType.Data)
                {
                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += (s, args) => {
                        editEventType.Text = eq.Title;
                        eventTypeId = eq.Id;
                        editName.Focus();
                    };
                    var label = new Label
                    { 
                        Text = eq.Title
                    };
                    label.GestureRecognizers.Add(tapGestureRecognizer);
                    stackEventType.Children.Add(label);
                    if (stackEventType.Children.Count >= 5)
                        break;
                }

            }
            catch (Exception ex)
            {
                stackEventType.Children.Add(new Label
                {
                    Text = ex.Message,
                });
            }
        }

        async void btnSave_Clicked(object sender, EventArgs e)
        {
            Hide();
            if (!await CheckAllFieldsForNull())
                return;

            // If every thing is Ok - create event
            CreateEvent();
        }

        async void CreateEvent()
        {
            try
            {
                var eventView = new EventCreateRequest
                {
                    EventTypeId = eventTypeId,
                    Address = editAddress.Text,
                    Description = editDescription.Text,
                    Title = editName.Text,
                    Shifts = ShiftCreate
                };
                var jsonContent = JsonConvert.SerializeObject(eventView);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var result = await client.PostAsync("event/", content);

                var resultContent = await result.Content.ReadAsStringAsync();
                var message = JsonConvert.DeserializeObject<OneObjectResponse<EventView>>(resultContent);
                if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {message.StatusCode}");

                await DisplayAlert("", Resource.ADMIN_Updated, "Ok");

                await Navigation.PushAsync(new OneEventPage(message.Data.Id));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        async Task<bool> CheckAllFieldsForNull()
        {
            // Event type
            if (eventTypeId == Guid.Empty)
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
            if (stackShift.Children.Count <= 0)
            {
                await DisplayAlert("Error", $"Please add '{Resource.Shifts}'", "Ok");
                return false;
            }

            return true;
        }

        async void btnAddShift_Clicked(object sender, EventArgs e)
        {
            Hide();
            ShiftCreateRequest shiftCreateRequest = await new Popup.Event.CreateShift().AddShift(Navigation);
            if (shiftCreateRequest == null)
                return;

            ShiftCreate.Add(shiftCreateRequest);
            // TODO: show shifts
        }

        void Show()
            => stackEventType.IsVisible = true;

        void Hide()
            => stackEventType.IsVisible = false;

        void editName_Focused(object sender, FocusEventArgs e)
            => Hide();

        void UpdateLanguage()
        {
            Title = Resource.TitleCreateEvent;
            lblEventType.Text = Resource.EventType;
            lblName.Text = Resource.Name;
            lblDescription.Text = Resource.Description;
            lblAddress.Text = Resource.Address;
            ///////////////////////////////////////
            btnAddShift.Text = Resource.Create + " " + Resource.Shifts;
            btnSave.Text = Resource.Save;
            ///////////////////////////////////////
            editEventType.Placeholder = lblEventType.Text;
            editName.Placeholder = lblName.Text;
            editDescription.Placeholder = lblDescription.Text;
            editAddress.Placeholder = lblAddress.Text;
        }
    }
}