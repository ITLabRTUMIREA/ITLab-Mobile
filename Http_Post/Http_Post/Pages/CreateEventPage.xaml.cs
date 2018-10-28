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
        private HttpClient client = Services.HttpClientFactory.HttpClient;
        private EventTypeView newETV;
        private Guid eventId;
        private List<ShiftEditRequest> ShiftEdit = new List<ShiftEditRequest>();
        private List<ShiftCreateRequest> ShiftCreates = new List<ShiftCreateRequest>();
        private bool isCreating;

        public CreateEventPage(EventViewExtended Event)
        {
            Init(false);
            eventId = Event.Id; // set id
            // TODO: place equipment and invited
            for (int i = 0; i < Event.Shifts.Count; i++)
            {
                ShiftEdit.Add(new ShiftEditRequest
                {
                    Id = Event.Shifts[i].Id,
                    BeginTime = Event.Shifts[i].BeginTime,
                    EndTime = Event.Shifts[i].EndTime,
                    Description = Event.Shifts[i].Description,
                    Places = new List<PlaceEditRequest>()
                });
                for (int j = 0; j < Event.Shifts[i].Places.Count; j++)
                {
                    ShiftEdit[i].Places.Add(new PlaceEditRequest
                    {
                        Id = Event.Shifts[i].Places[j].Id,
                        Description = Event.Shifts[i].Places[j].Description,
                        TargetParticipantsCount = Event.Shifts[i].Places[j].TargetParticipantsCount,
                    });
                }
            } // cast from ShiftView to ShiftEditRequest
            listShifts.ItemsSource = ShiftEdit;
            listShifts.IsVisible = true; // make it visible
            newETV = Event.EventType; // set event type

            editEventType.Text = Event.EventTypeTitle; // event type title
            editName.Text = Event.EventTitle; // event title
            editDescription.Text = Event.EventDescription; // event description
            editAddress.Text = Event.EventAddress; // event address
        }

        public CreateEventPage ()
		{
            Init(true);
		}

        private void Init(bool creating)
        {
            InitializeComponent();
            UpdateLanguage();
            isCreating = creating;
        }

        private async void editEventType_TextChanged(object sender, TextChangedEventArgs e)
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
                        newETV = eq;
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

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            Hide();
            if (!await CheckAllFieldsForNull())
                return;

            // If every thing is Ok - create event
            CreateEvent();
        }

        private async void CreateEvent()
        {
            try
            {
                EventCreateRequest eventCreate = null;
                EventEditRequest eventView = null;
                if (isCreating)
                    eventCreate = new EventCreateRequest
                    {
                        EventTypeId = newETV.Id,
                        Address = editAddress.Text,
                        Description = editDescription.Text,
                        Title = editName.Text,
                        Shifts = ShiftCreates // TODO: shift logic
                    };
                else
                    eventView = new EventEditRequest
                    {
                        Id = eventId,
                        Title = editName.Text,
                        Description = editDescription.Text,
                        Address = editAddress.Text,
                        //EventType = newETV,
                        Shifts = null // TODO: shift logic
                    };

                var jsonContent = isCreating ?
                    JsonConvert.SerializeObject(eventCreate) : JsonConvert.SerializeObject(eventView);
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
            if (listShifts.ItemsSource == null)
            //if (stackShifts.Children.Count <= 0)
            {
                await DisplayAlert("Error", $"Please add '{Resource.Shifts}'", "Ok");
                return false;
            }

            return true;
        }

        private async void btnCreateEventType_Clicked(object sender, EventArgs e)
        {
            await AddEventType(Navigation);
        }

        public Task<string> AddEventType(INavigation navigation)
        { 
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
                var entryTitle = new Editor { Text = editEventType.Text, // if no such title - add it to new event type
                    Placeholder = Resource.EventType,
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

                    var newEventType = new EventTypeView
                    {
                        Title = entryTitle.Text,
                        Description = entryDescription.Text,
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
                    Text = Resource.ADMIN_Cancel,
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

                layout.Children.Add(lbl);
                layout.Children.Add(entryTitle);
                layout.Children.Add(entryDescription);
                layout.Children.Add(slButtons);

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

        private async void btnAddShift_Clicked(object sender, EventArgs e)
        {
            Hide();
            await AddShift(Navigation);
        }

        public Task<string> AddShift(INavigation navigation)
        {
            var tcs = new TaskCompletionSource<string>();
            var grid = new Grid
            {
                Padding = new Thickness(0, 40, 0, 0),
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Style = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Stack"] as Style,
            }; // init here in a way to use it in 'catch' block
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            try
            {
                var th = new Classes.ThemeChanger().Theme;
                Style styleLbl = Application.Current.Resources[th + "_Lbl"] as Style;
                Style styleBtn = Application.Current.Resources[th + "_Btn"] as Style;
                Style styleStack = Application.Current.Resources[th + "_Stack"] as Style;

                var lbl = new Label
                {
                    Text = Resource.Create,
                    HorizontalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold,
                    Style = styleLbl
                };
                #region Adding labels which will show user, where is beginig and ending time/date
                var lblBegin = new Label
                {
                    Text = "Begin", // TODO: localize??
                    HorizontalOptions = LayoutOptions.Center,
                    Style = styleLbl
                };
                var lblEnd = new Label
                {
                    Text = "End",
                    HorizontalOptions = LayoutOptions.Center,
                    Style = styleLbl
                };
                #endregion
                var entryDescription = new Editor
                {
                    Text = "",
                    Placeholder = Resource.Description,
                    Style = styleLbl,
                };
                #region Adding time/date pickers and create special layouts for them
                var beginDate = new DatePicker
                {
                    MinimumDate = DateTime.Now.AddDays(-3)
                };
                var endDate = new DatePicker
                {
                    MinimumDate = DateTime.Now.AddDays(-3)
                };
                var beginTime = new TimePicker { Format = "HH:mm" };
                var endTime = new TimePicker();
                #endregion
                #region Adding places label hints and places editros
                var lblPlaces = new Label
                {
                    Text = "Places quantity",
                    HorizontalOptions = LayoutOptions.Center,
                    Style = styleLbl
                };
                var lblPeople = new Label
                {
                    Text = "People quantity",
                    HorizontalOptions = LayoutOptions.Center,
                    Style = styleLbl
                };
                var editPlaces = new Editor
                {
                    Text = "",
                    Placeholder = lblPlaces.Text,
                    Keyboard = Keyboard.Numeric,
                    Style = styleLbl
                };
                var editPeople = new Editor
                {
                    Text = "",
                    Placeholder = lblPeople.Text,
                    Keyboard = Keyboard.Numeric,
                    Style = styleLbl
                };
                #endregion
                var btnOk = new Button
                {
                    Text = "Ok",
                    WidthRequest = 100,
                    Style = styleBtn
                };
                btnOk.Clicked += async (s, e) =>
                {
                    try
                    {
                        if (endDate.Date < beginDate.Date)
                            throw new Exception($"Error: {Resource.ErrorNoDate}"); // Ending date can't be less than begining date!

                        if (isCreating)
                        {
                            var placesCreate = new List<PlaceCreateRequest>();
                            for (int i = 0; i < Convert.ToInt32(editPlaces.Text); i++)
                                placesCreate.Add(new PlaceCreateRequest
                                {
                                    TargetParticipantsCount = Convert.ToInt32(editPeople.Text)
                                });
                            
                            var newShift = new ShiftCreateRequest
                            {
                                Description = entryDescription.Text,
                                BeginTime = new DateTime(
                                beginDate.Date.Year,
                                beginDate.Date.Month,
                                beginDate.Date.Day,
                                beginTime.Time.Hours,
                                beginTime.Time.Minutes,
                                beginTime.Time.Seconds
                                ),
                                EndTime = new DateTime(
                                endDate.Date.Year,
                                endDate.Date.Month,
                                endDate.Date.Day,
                                endTime.Time.Hours,
                                endTime.Time.Minutes,
                                endTime.Time.Seconds
                                ),
                                Places = placesCreate
                            };
                            ShiftCreates.Add(newShift);
                            listShifts.ItemsSource = ShiftCreates;
                        }
                        else
                        {
                            var placesEdit = new List<PlaceEditRequest>();
                            for (int i = 0; i < Convert.ToInt32(editPlaces.Text); i++)
                                placesEdit.Add(new PlaceEditRequest
                                {
                                    TargetParticipantsCount = Convert.ToInt32(editPeople.Text),
                                });

                            var shiftEdit = new ShiftEditRequest
                            {
                                Description = entryDescription.Text,
                                BeginTime = new DateTime(
                                beginDate.Date.Year,
                                beginDate.Date.Month,
                                beginDate.Date.Day,
                                beginTime.Time.Hours,
                                beginTime.Time.Minutes,
                                beginTime.Time.Seconds
                                ),
                                EndTime = new DateTime(
                                endDate.Date.Year,
                                endDate.Date.Month,
                                endDate.Date.Day,
                                endTime.Time.Hours,
                                endTime.Time.Minutes,
                                endTime.Time.Seconds
                                ),
                                Places = placesEdit
                            };
                            ShiftEdit.Add(shiftEdit);
                            listShifts.ItemsSource = ShiftEdit;
                        }
                        await navigation.PopModalAsync();
                        listShifts.IsVisible = true;
                        tcs.SetResult(null);
                    }
                    catch(FormatException ex)
                    {
                        await DisplayAlert("Error", "Enter only numbrers '0-9'", "Ok");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", ex.Message, "Ok");
                    }
                };

                var btnCancel = new Button
                {
                    Text = Resource.ADMIN_Cancel,
                    WidthRequest = 100,
                    Style = styleBtn
                };
                btnCancel.Clicked += async (s, e) =>
                {
                    await navigation.PopModalAsync();
                    tcs.SetResult(null);
                };
                #region adding views to grid
                grid.Children.Add(lbl, 0, 0);
                Grid.SetColumnSpan(lbl, 2);
                grid.Children.Add(entryDescription, 0, 1);
                Grid.SetColumnSpan(entryDescription, 2);
                grid.Children.Add(lblBegin, 0, 2);
                grid.Children.Add(lblEnd, 1, 2);
                grid.Children.Add(beginDate, 0, 3);
                grid.Children.Add(endDate, 1, 3);
                grid.Children.Add(beginTime, 0, 4);
                grid.Children.Add(endTime, 1, 4);
                grid.Children.Add(lblPlaces, 0, 5);
                grid.Children.Add(lblPeople, 1, 5);
                grid.Children.Add(editPlaces, 0, 6);
                grid.Children.Add(editPeople, 1, 6);
                grid.Children.Add(btnOk, 0, 7);
                grid.Children.Add(btnCancel, 1, 7);
                #endregion
                var page = new ContentPage()
                {
                    Style = styleStack
                };
                page.Content = grid;
                navigation.PushModalAsync(page);
                entryDescription.Focus();
                return tcs.Task;
            }
            catch (Exception ex)
            {
                var itisLabel = grid.Children[grid.Children.Count - 1];
                if (itisLabel.Equals(new Label()))
                {
                    ((Label)grid.Children[grid.Children.Count - 1]).Text = ex.Message;
                }
                else
                {
                    grid.Children.Add(new Label
                    {
                        Text = ex.Message,
                        Style = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Lbl"] as Style,
                        HorizontalOptions = LayoutOptions.Center
                    }, 0, 8);
                }
                return tcs.Task; // while Task != "good" -> invoke Task
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

        private void UpdateLanguage()
        {
            Title = Resource.TitleCreateEvent;
            lblEventType.Text = Resource.EventType;
            lblName.Text = Resource.Name;
            lblDescription.Text = Resource.Description;
            lblAddress.Text = Resource.Address;
            ///////////////////////////////////////
            btnAddShift.Text = Resource.Shifts;
            btnSave.Text = Resource.Save;
            btnCreateEventType.Text = Resource.Create + " " + Resource.EventType;
            ///////////////////////////////////////
            editEventType.Placeholder = lblEventType.Text;
            editName.Placeholder = lblName.Text;
            editDescription.Placeholder = lblDescription.Text;
            editAddress.Placeholder = lblAddress.Text;
        }

        private void listShifts_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // TODO: load one shift page; isChanging - true
            if (isCreating)
                Navigation.PushAsync(new OneShiftViewPage((ShiftCreateRequest)sender));
            else
                Navigation.PushAsync(new OneShiftViewPage((ShiftEditRequest)sender));
        }
    }
}