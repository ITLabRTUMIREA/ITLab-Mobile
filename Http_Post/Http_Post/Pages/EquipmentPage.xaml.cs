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
using Xamarin.Forms.Xaml;

namespace Http_Post.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EquipmentPage : ContentPage
	{
        HttpClient client = HttpClientFactory.HttpClient;
        IEnumerable<CompactEquipmentViewExtended> listEquip = new List<CompactEquipmentViewExtended>();

		public EquipmentPage ()
		{
            Init();
            GetEquipment();


            // TODO: fix bug - not enough space
            listView.Refreshing += (s, e) => {
                GetEquipment();
                listView.IsRefreshing = false;
            };
            ChangeToolBar();
        }

        void Init()
        {
            InitializeComponent();
            UpdateLanguage();

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                Label_Type.FontAttributes = FontAttributes.None;
                Label_Owner.FontAttributes = FontAttributes.None;
                Label_Number.FontAttributes = FontAttributes.None;
                var lbl = (Label)s;
                if (lbl.Equals(Label_Type))
                {
                    listView.ItemsSource = listEquip.OrderBy(se => se.EquipmentType.Title);
                    Label_Type.FontAttributes = FontAttributes.Bold;
                }
                else if (lbl.Equals(Label_Owner))
                {
                    listView.ItemsSource = listEquip.OrderBy(se => se.OwnerName);
                    Label_Owner.FontAttributes = FontAttributes.Bold;
                }
                else if (lbl.Equals(Label_Number))
                {
                    listView.ItemsSource = listEquip.OrderBy(se => se.Number);
                    Label_Number.FontAttributes = FontAttributes.Bold;
                }
            };
            Label_Type.GestureRecognizers.Add(tapGestureRecognizer);
            Label_Owner.GestureRecognizers.Add(tapGestureRecognizer);
            Label_Number.GestureRecognizers.Add(tapGestureRecognizer);
        }

        async void GetEquipment()
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
                Label_Number.FontAttributes = FontAttributes.Bold;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }
        
        async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var equip = (CompactEquipmentViewExtended)e.Item;
            await Navigation.PushAsync(new OneEquipmentPage(equip.Id, GetRight()));
        }

        void UpdateLanguage()
        {
            Title = Device.RuntimePlatform == Device.UWP ? Resource.TitleEquipment : "";
            Label_Type.Text = Resource.EquipmentType;
            Label_Owner.Text = Resource.Owner;
            Label_Number.Text = Resource.Number;
        }

        void ChangeToolBar()
        {
            if (!GetRight())

                return;

            var itemChange = new ToolBar.ToolBarItems().Item(null, 1, ToolbarItemOrder.Primary, "CreateCircle.png");
            itemChange.Clicked += async(s,e) =>
            {
                await Navigation.PushAsync(new CreateEquipment());
            };
            ToolbarItems.Add(itemChange);
        }

        bool GetRight()
        {
            string whatToCheck = "CanEditEquipment";
            foreach (var item in CurrentUserIdFactory.UserRoles)
                if (item.Equals(whatToCheck))
                    return true;
            return false;
        }
    }
}