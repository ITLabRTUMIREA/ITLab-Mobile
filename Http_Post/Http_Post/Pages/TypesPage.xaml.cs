using Http_Post.Res;
using Http_Post.Services;
using Models.PublicAPI.Requests.Equipment.EquipmentType;
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
		public TypesPage ()
		{
			InitializeComponent ();
            UpdateLanguage();

            InitTapGestures();

            GetEventTypes(); // by default
            lblEventTypes.FontAttributes = FontAttributes.Bold;
		}

        private void InitTapGestures()
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sender, e) =>
            {
                var lbl = (Label)sender;
                // if tapped event types
                if (lbl.Equals(lblEventTypes))
                {
                    lblEventTypes.FontAttributes = FontAttributes.Bold;
                    lblRoles.FontAttributes = FontAttributes.None;
                    lblEquipmentTypes.FontAttributes = FontAttributes.None;
                    GetEventTypes();
                }
                // if tapped roles
                else if (lbl.Equals(lblRoles))
                {
                    lblEventTypes.FontAttributes = FontAttributes.None;
                    lblRoles.FontAttributes = FontAttributes.Bold;
                    lblEquipmentTypes.FontAttributes = FontAttributes.None;
                    GetRoles();
                }
                // if tapped equipment types
                else if (lbl.Equals(lblEquipmentTypes))
                {
                    lblEventTypes.FontAttributes = FontAttributes.None;
                    lblRoles.FontAttributes = FontAttributes.None;
                    lblEquipmentTypes.FontAttributes = FontAttributes.Bold;
                    GetEquipmentTypes();
                }
            };
            lblEventTypes.GestureRecognizers.Add(tapGestureRecognizer);
            lblRoles.GestureRecognizers.Add(tapGestureRecognizer);
            lblEquipmentTypes.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private async void GetEventTypes()
        {
            var response = await client.GetStringAsync("EventType?all=true");
            var rowTypes = JsonConvert.DeserializeObject<ListResponse<EventTypeView>>(response);

            listView.ItemsSource = rowTypes.Data;

        }

        private async void GetRoles()
        {
            var response = await client.GetStringAsync("eventrole");
            var rowTypes = JsonConvert.DeserializeObject<ListResponse<EventRoleView>>(response);

            listView.ItemsSource = rowTypes.Data;
        }

        private async void GetEquipmentTypes()
        {
            var response = await client.GetStringAsync("EquipmentType?all=true");
            var rowTypes = JsonConvert.DeserializeObject<ListResponse<CompactEquipmentTypeView>>(response);

            listView.ItemsSource = rowTypes.Data;
        }

        void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // TODO: make separate pop ups file
        }

        void btnCreate_Clicked(object sender, EventArgs e)
        {
            // TODO: detect on which page we are now and then send CreateRequest
        }

        private void UpdateLanguage()
        {
            Title = Resource.TitleTypes;
            lblEventTypes.Text = Resource.EventType;
            lblRoles.Text = "Roles";
            lblEquipmentTypes.Text = Resource.EquipmentType;
            btnCreate.Text = Resource.Create;
        }
	}
}