using Http_Post.Extensions.Responses.Event;
using Http_Post.Res;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Requests.Events.Event.Create;
using Models.PublicAPI.Requests.Events.Event.Edit;
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
using Xamarin.Forms.Xaml;

namespace Http_Post.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateEventPage : ContentPage
	{
        HttpClient client = Services.HttpClientFactory.HttpClient;
        List<ShiftEditRequest> ShiftEdit;
        EventViewExtended _event;
        bool _isCreating;

        public CreateEventPage(EventViewExtended eventViewExtended)
        {
            Init(false);
            _event = eventViewExtended;

            editName.Text = _event.Title;
            editEventType.Text = _event.EventType.Title;
            Hide();
            editDescription.Text = _event.Description;
            editAddress.Text = _event.Address;

            ShiftEdit = GetFromNormalShifts(_event.Shifts);
            ShowShifts();
        }

        List<ShiftEditRequest> GetFromNormalShifts(List<ShiftView> shifts)
        {
            List<ShiftEditRequest> shiftEdits = new List<ShiftEditRequest>();
            foreach (var shift in shifts)
            {
                List<PlaceEditRequest> new_places = new List<PlaceEditRequest>(); // places
                foreach (var pl in shift.Places)
                {
                    List<PersonWorkRequest> inv = new List<PersonWorkRequest>(); // users
                    #region add users to list
                    foreach (var p in pl.Invited)
                        inv.Add(new PersonWorkRequest
                        {
                            Id = p.User.Id,
                            EventRoleId = p.EventRole.Id,
                        });
                    foreach (var p in pl.Wishers)
                        inv.Add(new PersonWorkRequest
                        {
                            Id = p.User.Id,
                            EventRoleId = p.EventRole.Id,
                        });
                    foreach (var p in pl.Participants)
                        inv.Add(new PersonWorkRequest
                        {
                            Id = p.User.Id,
                            EventRoleId = p.EventRole.Id,
                        });
                    foreach (var p in pl.Unknowns)
                        inv.Add(new PersonWorkRequest
                        {
                            Id = p.User.Id,
                            EventRoleId = p.EventRole.Id,
                        });
                    #endregion
                    List<DeletableRequest> equip = new List<DeletableRequest>(); // equip
                    foreach (var e in pl.Equipment)
                        equip.Add(new DeletableRequest
                        {
                            Id = e.Id
                        });
                    new_places.Add(new PlaceEditRequest
                    {
                        Id = pl.Id,
                        TargetParticipantsCount = pl.TargetParticipantsCount,
                        Description = pl.Description,
                        Invited = inv,
                        Equipment = equip
                    });
                }
                shiftEdits.Add(new ShiftEditRequest
                {
                    Id = shift.Id,
                    Description = shift.Description,
                    BeginTime = shift.BeginTime,
                    EndTime = shift.EndTime,
                    Places = new_places,
                });
            }
            return shiftEdits;
        }

        public CreateEventPage ()
		{
            _event = new EventViewExtended();
            _event.EventType = new EventTypeView();
            Init(true);
        }
        
        void Init(bool isCreating)
        {
            _isCreating = isCreating;
            ShiftEdit = new List<ShiftEditRequest>();
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
                        _event.EventType.Id = eq.Id;
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
                string jsonContent = "";
                if (_isCreating)
                {
                    EventCreateRequest eventCreateRequest = new EventCreateRequest
                    {
                        Title = editName.Text,
                        EventTypeId = _event.EventType.Id,
                        Description = editDescription.Text,
                        Address = editAddress.Text,
                        Shifts = GetFromEditToCreateShifts()
                    };
                    jsonContent = JsonConvert.SerializeObject(eventCreateRequest);
                }
                else
                {
                    EventEditRequest eventEditRequest = new EventEditRequest
                    {
                        Id = _event.Id,
                        EventTypeId = _event.EventType.Id,
                        Title = editName.Text,
                        Address = editAddress.Text,
                        Description = editDescription.Text,
                        Shifts = ShiftEdit
                    };
                    jsonContent = JsonConvert.SerializeObject(eventEditRequest);
                }

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var result = _isCreating ? await client.PostAsync("event/", content) : await client.PutAsync("event/", content);

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

        List<ShiftCreateRequest> GetFromEditToCreateShifts()
        {
            List<ShiftCreateRequest> shiftCreateRequests = new List<ShiftCreateRequest>();
            foreach(var shift in ShiftEdit)
            {
                List<PlaceCreateRequest> placeCreateRequests = new List<PlaceCreateRequest>();
                foreach(var place in shift.Places)
                {
                    List<PersonWorkRequest> inv = new List<PersonWorkRequest>();
                    foreach (var invited in place.Invited)
                        inv.Add(new PersonWorkRequest
                        {
                            Id = invited.Id,
                            EventRoleId = invited.EventRoleId,
                            Delete = invited.Delete
                        });
                    List<Guid> equipment = new List<Guid>();
                    foreach (var equip in place.Equipment)
                        equipment.Add(equip.Id);
                    placeCreateRequests.Add(new PlaceCreateRequest
                    {
                        TargetParticipantsCount = place.TargetParticipantsCount,
                        Description = place.Description,
                        Equipment = equipment,
                        Invited = inv
                    });
                }
                shiftCreateRequests.Add(new ShiftCreateRequest
                {
                    BeginTime = shift.BeginTime.Value,
                    EndTime = shift.EndTime.Value,
                    Description = shift.Description,
                    Places = placeCreateRequests
                });
            }
            return shiftCreateRequests;
        }

        async Task<bool> CheckAllFieldsForNull()
        {
            // Event type
            if (_event.EventType.Id == Guid.Empty)
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
            ShiftEditRequest shiftEditRequest = await new Popup.Event.CreateShift().AddShiftEditRequest(Navigation);
            if (shiftEditRequest == null)
                return;

            ShiftEdit.Insert(0, shiftEditRequest);
            ShowShifts();
        }

        void ShowShifts()
        {
            stackShift.Children.Clear();
            int number = 1;
            foreach (var shift in ShiftEdit.Reverse<ShiftEditRequest>())
            {
                if (shift.Delete)
                    continue;
                stackShift.Children.Add(new Controls.ShiftEditContentView(shift, number));
                number++;
            }
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