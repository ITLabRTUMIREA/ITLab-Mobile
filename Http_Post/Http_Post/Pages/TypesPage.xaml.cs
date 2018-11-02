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
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Http_Post.Pages
{
    public partial class TypesPage : ContentPage
    {
        HttpClient client = HttpClientFactory.HttpClient;

        enum Types
        {
            Event,
            Role,
            Equipment
        }
        private Types types;

        public TypesPage()
        {
            InitializeComponent();
            UpdateLanguage();

            InitTapGestures();

            types = Types.Event; // by default
            ChooseList();
        }

        private void InitTapGestures()
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sender, e) =>
            {
                var lbl = (Label)sender;
                // if tapped event types
                if (lbl.Equals(lblEventTypes))
                    types = Types.Event;
                // if tapped roles
                else if (lbl.Equals(lblRoles))
                    types = Types.Role;
                // if tapped equipment types
                else if (lbl.Equals(lblEquipmentTypes))
                    types = Types.Equipment;

                ChooseList();
            };
            lblEventTypes.GestureRecognizers.Add(tapGestureRecognizer);
            lblRoles.GestureRecognizers.Add(tapGestureRecognizer);
            lblEquipmentTypes.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void ChooseList()
        {
            lblEventTypes.FontAttributes = FontAttributes.None;
            lblRoles.FontAttributes = FontAttributes.None;
            lblEquipmentTypes.FontAttributes = FontAttributes.None;

            switch (types)
            {
                case Types.Event:
                    { 
                        lblEventTypes.FontAttributes = FontAttributes.Bold;
                        GetEventTypes();
                    }
                    break;
                case Types.Role:
                    {
                        lblRoles.FontAttributes = FontAttributes.Bold;
                        GetRoles();
                    }
                    break;
                case Types.Equipment:
                    {
                        lblEquipmentTypes.FontAttributes = FontAttributes.Bold;
                        GetEquipmentTypes();
                    }
                    break;
            }
        }

        private async void GetEventTypes()
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

        private async void GetRoles()
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

        private async void GetEquipmentTypes()
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
                            jsonContent = JsonConvert.SerializeObject(newEquipmentType);
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
                            var message = JsonConvert.DeserializeObject<OneObjectResponse<EquipmentTypeView>>(content);
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

        private void UpdateLanguage()
        {
            Title = Resource.TitleTypes;
            lblEventTypes.Text = Resource.EventType;
            lblRoles.Text = Resource.Roles;
            lblEquipmentTypes.Text = Resource.EquipmentType;
            btnCreate.Text = Resource.Create;
        }
    }
}