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
	public partial class CreateEquipment : ContentPage
	{
        private HttpClient client = HttpClientFactory.HttpClient;

        public CreateEquipment ()
		{
			InitializeComponent ();

            UpdateLanguage();
		}

        private async void EntryEquipType_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // TODO: replace with stacklayout
                listView.IsVisible = true;
                var lbl = (Entry)sender;
                var response = await client.GetStringAsync($"EquipmentType?match={lbl.Text}");
                var equipType = JsonConvert.DeserializeObject<ListResponse<EquipmentTypeView>>(response);
                if (equipType.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {equipType.StatusCode}");

                listView.ItemsSource = equipType.Data;
            }
            catch (Exception ex)
            {

            }
        }

        private void UpdateLanguage()
        {
            Title = Resource.Title_Create;
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Entry_EquipType.Text = ((EquipmentTypeView)e.Item).Title;
            listView.IsVisible = false;
            Entry_SerialNumber.Focus();
        }

        // TODO: Add small button to create new Equipment Type!!!!!!!!!!!!!!!!!!!!!!!
    }
}