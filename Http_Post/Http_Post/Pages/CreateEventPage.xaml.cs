using Http_Post.Extensions.Responses.Event;
using Http_Post.Res;
using Models.PublicAPI.Requests.Events.Event.Create;
using Models.PublicAPI.Requests.Events.Event.Edit;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Http_Post.Pages
{
	public partial class CreateEventPage : ContentPage
	{
        HttpClient client = Services.HttpClientFactory.HttpClient;
        Guid eventTypeId;
        Guid eventId;
        List<ShiftEditRequest> ShiftEdit = new List<ShiftEditRequest>();
        List<ShiftCreateRequest> ShiftCreate = new List<ShiftCreateRequest>();
        bool isCreating;

        public CreateEventPage(EventViewExtended Event)
        {
            Init(false);
            eventId = Event.Id; // set id
            eventTypeId = Event.EventType.Id;

            foreach (var shift in Event.Shifts)
            {
                var places = new List<PlaceEditRequest>();
                foreach (var place in shift.Places)
                {
                    var users = new List<PersonWorkRequest>();
                    foreach (var user in place.Participants)
                        users.Add(new PersonWorkRequest
                        {
                            EventRoleId = user.EventRole.Id
                        });

                    places.Add(new PlaceEditRequest
                    {
                        Id = place.Id,
                        Description = place.Description,
                        TargetParticipantsCount = place.TargetParticipantsCount,
                        Invited = users
                    });
                }
                ShiftEdit.Add(new ShiftEditRequest
                {
                    Id = shift.Id,
                    BeginTime = shift.BeginTime,
                    EndTime = shift.EndTime,
                    Description = shift.Description,
                    Places = places
                });
            }

            //stackShift.Children = new Controls.StackShiftView(Event.Shifts, true).stackLayout.Children;
            listShift.IsVisible = true;
            var tempList = new List<Controls.ShiftsView>();
            int ShiftNumber = 1;
            foreach (var shift in Event.Shifts)
            {
                tempList.Add(new Controls.ShiftsView(shift, ShiftNumber, true));
                ShiftNumber++;
            }
            listShift.ItemsSource = tempList;
            //stackShift.Children.Add(new Controls.StackShiftView(Event.Shifts, true).stackLayout);
        }

        public CreateEventPage ()
		{
            Init(true);
		}

        void Init(bool creating)
        {
            InitializeComponent();
            UpdateLanguage();
            isCreating = creating;
        }

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

                Style styleLbl = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Lbl"] as Style;
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
                        Style = styleLbl,
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
                    Style = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Lbl"] as Style
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
                string jsonContent = "";
                if (isCreating)
                {
                    var eventView = new EventCreateRequest
                    {
                        EventTypeId = eventTypeId,
                        Address = editAddress.Text,
                        Description = editDescription.Text,
                        Title = editName.Text,
                        Shifts = ShiftCreate
                    };
                    jsonContent = JsonConvert.SerializeObject(eventView);
                }
                else
                {
                    var eventView = new EventEditRequest
                    {
                        Id = eventId,
                        Title = editName.Text,
                        Description = editDescription.Text,
                        Address = editAddress.Text,
                        EventTypeId = eventTypeId,
                        Shifts = ShiftEdit
                    };
                    jsonContent = JsonConvert.SerializeObject(eventView);
                }

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var result = isCreating ?
                    await client.PostAsync("event/", content) : await client.PutAsync("event/", content);

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
            if (ShiftCreate.Count == 0 || ShiftEdit.Count == 0)
            //if (stackShifts.Children.Count <= 0)
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

            if (isCreating)
                ShiftCreate.Add(shiftCreateRequest);
            else
            {
                ShiftEditRequest shiftEditRequest = FromCreateToEdit(shiftCreateRequest);
                ShiftEdit.Add(shiftEditRequest);
            }
        }

        ShiftEditRequest FromCreateToEdit(ShiftCreateRequest shift)
        {
            var places = new List<PlaceEditRequest>();
            foreach(var place in shift.Places)
                places.Add(new PlaceEditRequest
                {
                    TargetParticipantsCount = place.TargetParticipantsCount
                });

            return new ShiftEditRequest
            {
                BeginTime = shift.BeginTime,
                EndTime = shift.EndTime,
                Description = shift.Description,
                Places = places
            };
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