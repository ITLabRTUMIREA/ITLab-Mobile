using Http_Post.Extensions.Responses.Event;
using Http_Post.Res;
using Http_Post.Services;
using Models.PublicAPI.Responses.Equipment;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;
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
	public partial class EquipmentPage : ContentPage
	{
        HttpClient client = HttpClientFactory.HttpClient;

		public EquipmentPage ()
		{
			InitializeComponent ();
            UpdateLanguage();

            GetEquipment();
		}

        public async void GetEquipment()
        {
            try
            {
                var response = await client.GetStringAsync("Equipment/");
                var equip = JsonConvert.DeserializeObject<ListResponse<EquipmentViewExtended>>(response);

                if (equip.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: " + equip.StatusCode);

                foreach(var e in equip.Data)
                {
                    if (e.OwnerId == null)
                    {
                        e.OwnerName = Resource.ADMIN_Laboratory;
                        continue;
                    }

                    var userRaw = await client.GetStringAsync($"user/{e.OwnerId}");
                    var user = JsonConvert.DeserializeObject<OneObjectResponse<UserView>>(userRaw);
                    e.OwnerName = user.Data.FirstName + " " + user.Data.LastName;
                }
                    
                listView.ItemsSource = equip.Data;
            } catch(Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }
        
        private async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var equip = (EquipmentViewExtended)e.Item;
            await Navigation.PushAsync(new OneEquipmentPage(equip.Id));
            GetEquipment();
        }

        private void UpdateLanguage()
        {
            Title = Resource.Title_Equipment;
            Label_Type.Text = Resource.Equipment_Type;
            Label_Owner.Text = Resource.Equipment_Owner;
            Label_SerialNumber.Text = Resource.Equipment_SerialNumber;
        }
    }
}