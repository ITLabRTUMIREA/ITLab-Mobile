using Http_Post.Res;
using Http_Post.Services;
using Models.PublicAPI.Requests.Equipment.EquipmentType;
using Models.PublicAPI.Requests.Events.Event.Create.Roles;
using Models.PublicAPI.Requests.Events.Event.Edit.Roles;
using Models.PublicAPI.Requests.Events.EventType;
using Models.PublicAPI.Responses.Equipment;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TypesPage : ContentPage
    {
        HttpClient client = HttpClientFactory.HttpClient;

        enum Types
        {
            Event,
            Role,
            Equipment
        }
        Types types;

        public TypesPage()
        {
            InitializeComponent();
            Title = Device.RuntimePlatform == Device.UWP ? Resource.TitleTypes : "";
            btnEvent.Text = Resource.Event;
            btnRoles.Text = Resource.Role;
            btnEquipment.Text = Resource.TitleEquipment;

            listView.Refreshing += (s, e) => {
                ChooseList();
                listView.IsRefreshing = false;
            };
            types = Types.Event; // by default
            ChooseList();
        }

        void ChooseList()
        {
            btnEvent.FontAttributes = FontAttributes.None;
            btnEquipment.FontAttributes = FontAttributes.None;
            btnRoles.FontAttributes = FontAttributes.None;
            switch (types)
            {
                case Types.Event:
                    {
                        btnEvent.FontAttributes = FontAttributes.Bold;
                        GetEventTypes();
                    }
                    break;
                case Types.Role:
                    {
                        btnRoles.FontAttributes = FontAttributes.Bold;
                        GetRoles();
                    }
                    break;
                case Types.Equipment:
                    {
                        btnEquipment.FontAttributes = FontAttributes.Bold;
                        GetEquipmentTypes();
                    }
                    break;
            }
            btnCreate.IsVisible = GetRight();
        }

        void btnEvents_Clicked(object sender, EventArgs e)
        {
            types = Types.Event;
            ChooseList();
        }

        void btnEquipment_Clicked(object sender, EventArgs e)
        {
            types = Types.Equipment;
            ChooseList();
        }

        void btnRoles_Clicked(object sender, EventArgs e)
        {
            types = Types.Role;
            ChooseList();
        }

        async void GetEventTypes()
        {
            try
            {
                var response = await client.GetStringAsync("EventType?all=true");
                var rowTypes = JsonConvert.DeserializeObject<ListResponse<EventTypeView>>(response);

                listView.ItemsSource = rowTypes.Data;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        async void GetRoles()
        {
            try
            {
                var response = await client.GetStringAsync("eventrole");
                var rowTypes = JsonConvert.DeserializeObject<ListResponse<EventRoleView>>(response);

                listView.ItemsSource = rowTypes.Data;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        async void GetEquipmentTypes()
        {
            try
            {
                var response = await client.GetStringAsync("EquipmentType?all=true");
                var rowTypes = JsonConvert.DeserializeObject<ListResponse<CompactEquipmentTypeView>>(response);

                listView.ItemsSource = rowTypes.Data;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!GetRight())
                return;
            try
            {
                string url = "";
                string jsonContent = "";
                switch (types)
                {
                    case Types.Event:
                        {
                            var tapped = (EventTypeView)e.Item;
                            EventTypeEditRequest eventTypeEdit = new EventTypeEditRequest
                            {
                                Id = tapped.Id,
                                Title = tapped.Title,
                                Description = tapped.Description
                            };
                            var newEventType = await new Popup.Types.EditTypePage().eventTypeEdit(Navigation, eventTypeEdit);
                            if (newEventType == null)
                                return;
                            url = "EventType";
                            jsonContent = JsonConvert.SerializeObject(newEventType);
                        }
                        break;
                    case Types.Role:
                        {
                            var tapped = (EventRoleView)e.Item;
                            EventRoleEditRequest eventRoleEdit = new EventRoleEditRequest
                            {
                                Id = tapped.Id,
                                Title = tapped.Title,
                                Description = tapped.Description
                            };
                            var newEventRole = await new Popup.Types.EditTypePage().eventRoleEdit(Navigation, eventRoleEdit);
                            if (newEventRole == null)
                                return;
                            url = "eventrole";
                            jsonContent = JsonConvert.SerializeObject(newEventRole);
                        }
                        break;
                    case Types.Equipment:
                        {
                            var tapped = (CompactEquipmentTypeView)e.Item;
                            EquipmentTypeEditRequest equipmentTypeEdit = new EquipmentTypeEditRequest
                            {
                                Id = tapped.Id,
                                ShortTitle = tapped.ShortTitle,
                                Title = tapped.Title,
                                Description = tapped.Description,
                            };
                            var newEquipmentType = await new Popup.Types.EditTypePage().equipmentTypeEdit(Navigation, equipmentTypeEdit);
                            if (newEquipmentType == null)
                                return;
                            url = "EquipmentType";

                            jsonContent = JsonConvert.SerializeObject(new[] { newEquipmentType });
                        }
                        break;
                }

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var request = await client.PutAsync(url, content);

                var requestContent = await request.Content.ReadAsStringAsync();

                Unconvert(requestContent);
                ChooseList(); // update list
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        async void btnCreate_Clicked(object sender, EventArgs e)
        {
            try
            { 
                string url = "";
                string jsonContent = "";
                if (types == Types.Event)
                {
                    EventTypeCreateRequest eventType = await new Popup.Types.CreateTypePage().eventTypeCreate(Navigation);
                    if (eventType == null)
                        return;
                    url = "EventType";
                    jsonContent = JsonConvert.SerializeObject(eventType);
                }
                else if (types == Types.Role)
                {
                    EventRoleCreateRequest eventRole = await new Popup.Types.CreateTypePage().eventRoleCreate(Navigation);
                    if (eventRole == null)
                        return;
                    url = "eventrole";
                    jsonContent = JsonConvert.SerializeObject(eventRole);
                }
                else if (types == Types.Equipment)
                {
                    EquipmentTypeCreateRequest equipmentType = await new Popup.Types.CreateTypePage().equipmentTypeCreate(Navigation);
                    if (equipmentType == null)
                        return;
                    url = "EquipmentType";
                    jsonContent = JsonConvert.SerializeObject(equipmentType);
                }
            
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var request = await client.PostAsync(url, content);

                var requestContent = await request.Content.ReadAsStringAsync();

                Unconvert(requestContent);
                ChooseList(); // update list after cerating
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        async void Unconvert(string content)
        {
            try
            {
                switch (types)
                {
                    case Types.Event:
                        {
                            var message = JsonConvert.DeserializeObject<OneObjectResponse<EventTypeView>>(content);
                            if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                                throw new Exception($"Error: {message.StatusCode}");
                        }
                        break;
                    case Types.Role:
                        {
                            var message = JsonConvert.DeserializeObject<OneObjectResponse<EventRoleView>>(content);
                            if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                                throw new Exception($"Error: {message.StatusCode}");
                        }
                        break;
                    case Types.Equipment:
                        {
                            var message = JsonConvert.DeserializeObject<ListResponse<EquipmentTypeView>>(content);
                            if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                                throw new Exception($"Error: {message.StatusCode}");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        bool GetRight()
        {
            string whatToCheck = "";
            switch (types)
            {
                case Types.Event:
                    whatToCheck = "CanEditEventType";
                    break;
                case Types.Role:
                    whatToCheck = "CanEditRoles";
                    break;
                case Types.Equipment:
                    whatToCheck = "CanEditEquipmentType";
                    break;
            }
            foreach (var item in CurrentUserIdFactory.UserRoles)
                if (item.Equals(whatToCheck))
                    return true;
            return false;
        }

        void btnRefresh_BtnTapped(object sender, EventArgs e)
            => listView.BeginRefresh();
    }
}