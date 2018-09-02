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

            }
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }

        private void UpdateLanguage()
        {
            Title = Resource.Title_Equipment;
            Label_Type.Text = "Type (def)";
            Label_Owner.Text = "Owner (def)";
            Label_SerialNumber.Text = "Serial Number (def)";
        }
    }
}