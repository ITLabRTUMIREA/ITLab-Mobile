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
	public partial class EquipmentPage : ContentPage
	{
        HttpClient client = HttpClientFactory.HttpClient;

		public EquipmentPage ()
		{
			InitializeComponent ();
            UpdateLanguage();

            GetEquipment();
		}

        private async void GetEquipment()
        {
            try
            {
                var response = await client.GetStringAsync("Equipment/");
                var equip = JsonConvert.DeserializeObject<ListResponse<EquipmentView>>(response);
                listView.ItemsSource = equip.Data;
            } catch(Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // TODO: get owner name, change owner
            await DisplayAlert("Tapped", "How to get owner name?\nHow to change owner?", "Ok", "Cancel");
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