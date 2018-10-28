using Http_Post.Extensions.Responses.Event;
using Http_Post.Res;
using Http_Post.Services;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xamarin.Forms;

namespace Http_Post.Pages
{
	public partial class EquipmentPage : ContentPage
	{
        HttpClient client = HttpClientFactory.HttpClient;
        IEnumerable<CompactEquipmentViewExtended> listEquip = new List<CompactEquipmentViewExtended>();

		public EquipmentPage ()
		{
            Init();
            GetEquipment();
        }

        private void Init()
        {
            InitializeComponent();
            UpdateLanguage();

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                var lbl = (Label)s;
                if (lbl.Equals(Label_Type))
                    listView.ItemsSource = listEquip.OrderBy(se => se.EquipmentType.Title);
                else if (lbl.Equals(Label_Owner))
                    listView.ItemsSource = listEquip.OrderBy(se => se.OwnerName);
                else if (lbl.Equals(Label_Number))
                    listView.ItemsSource = listEquip.OrderBy(se => se.Number);
            };
            Label_Type.GestureRecognizers.Add(tapGestureRecognizer);
            Label_Owner.GestureRecognizers.Add(tapGestureRecognizer);
            Label_Number.GestureRecognizers.Add(tapGestureRecognizer);
        }

        public async void GetEquipment()
        {
            try
            {
                var response = await client.GetStringAsync("Equipment/");
                var equip = JsonConvert.DeserializeObject<ListResponse<CompactEquipmentViewExtended>>(response);

                if (equip.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: " + equip.StatusCode);

                foreach(var e in equip.Data)
                {
                    if (e.OwnerId == null)
                        e.OwnerName = Resource.ADMIN_Laboratory;
                    else
                    {
                        var userRaw = await client.GetStringAsync($"user/{e.OwnerId}");
                        var user = JsonConvert.DeserializeObject<OneObjectResponse<UserView>>(userRaw);
                        e.OwnerName = user.Data.FirstName + " " + user.Data.LastName;
                    }
                }

                listEquip = equip.Data;
                listView.ItemsSource = listEquip.OrderBy(s=>s.Number);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }
        
        private async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var equip = (CompactEquipmentViewExtended)e.Item;
            await Navigation.PushAsync(new OneEquipmentPage(equip.Id));
        }

        private void UpdateLanguage()
        {
            Title = Resource.TitleEquipment;
            Label_Type.Text = Resource.EquipmentType;
            Label_Owner.Text = Resource.Owner;
            Label_Number.Text = Resource.Number;
        }
    }
}