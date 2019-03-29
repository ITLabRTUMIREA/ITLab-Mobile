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
using System.Threading;
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
            InitializeComponent();
            this.IsEnabled = false;
            UpdateLanguage();
            UpdateFrames();
            GetEquipment();
            btnCreate.IsVisible = GetRight();

            listView.Refreshing += (s, e) => {
                GetEquipment();
                listView.IsRefreshing = false;
            };
            this.IsEnabled = true;
        }

        void UpdateFrames()
        {
            default_color = frameType.FrameColor;
            frameType.ImageLeft = Images.ImageManager.GetSourceImage("Types");
            frameType.FrameColor = Color.FromHex("#ff8080");
            frameOwner.ImageLeft = Images.ImageManager.GetSourceImage("Person");
            frameNumber.ImageLeft = Images.ImageManager.GetSourceImage("Number_1_to_3");
            frameOwner.ImageRight = frameNumber.ImageRight = frameType.ImageRight = Images.ImageManager.GetSourceImage("ArrowDown");
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

                listEquip = equip.Data.OrderBy(se => se.EquipmentType.Title);
                reserve = false;
                listView.ItemsSource = listEquip;
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
            Title = Device.RuntimePlatform == Device.Android ? "" : Resource.TitleEquipment;
        }

        bool GetRight()
        {
            string whatToCheck = "CanEditEquipment";
            foreach (var item in CurrentUserIdFactory.UserRoles)
                if (item.Equals(whatToCheck))
                    return true;
            return false;
        }

        bool reserve;
        Color default_color;
        void frameType_Clicked(object sender, EventArgs e)
        {
            reserve = frameType.FrameColor == Color.FromHex("#ff8080") ? !reserve : false;
            if (reserve)
            {
                listEquip = listEquip.OrderByDescending(se => se.EquipmentType.Title);
                frameType.ImageRight = Images.ImageManager.GetSourceImage("ArrowUp");
            }
            else
            {
                listEquip = listEquip.OrderBy(se => se.EquipmentType.Title);
                frameOwner.ImageRight = frameNumber.ImageRight = frameType.ImageRight = Images.ImageManager.GetSourceImage("ArrowDown");
            }

            frameType.FrameColor = frameOwner.FrameColor = frameNumber.FrameColor = default_color;
            frameType.FrameColor = Color.FromHex("#ff8080");
            listView.ItemsSource = listEquip;
        }

        void frameOwner_Clicked(object sender, EventArgs e)
        {
            reserve = frameOwner.FrameColor == Color.FromHex("#ff8080") ? !reserve : false;
            if (reserve)
            {
                listEquip = listEquip.OrderByDescending(se => se.OwnerName);
                frameOwner.ImageRight = Images.ImageManager.GetSourceImage("ArrowUp");
            }
            else
            {
                listEquip = listEquip.OrderBy(se => se.OwnerName);
                frameOwner.ImageRight = frameNumber.ImageRight = frameType.ImageRight = Images.ImageManager.GetSourceImage("ArrowDown");
            }

            frameType.FrameColor = frameOwner.FrameColor = frameNumber.FrameColor = default_color;
            frameOwner.FrameColor = Color.FromHex("#ff8080");
            listView.ItemsSource = listEquip;
        }

        void frameNumber_Clicked(object sender, EventArgs e)
        {
            reserve = frameNumber.FrameColor == Color.FromHex("#ff8080") ? !reserve : false;
            if (reserve)
            {
                listEquip = listEquip.OrderByDescending(se => se.Number);
                frameNumber.ImageRight = Images.ImageManager.GetSourceImage("ArrowUp");
            }
            else
            {
                listEquip = listEquip.OrderBy(se => se.Number);
                frameOwner.ImageRight = frameNumber.ImageRight = frameType.ImageRight = Images.ImageManager.GetSourceImage("ArrowDown");
            }

            frameType.FrameColor = frameOwner.FrameColor = frameNumber.FrameColor = default_color;
            frameNumber.FrameColor = Color.FromHex("#ff8080");
            listView.ItemsSource = listEquip;
        }

        async void btnCreate_Clicked(object sender, EventArgs e)
            => await Navigation.PushAsync(new CreateEquipment());

        void btnRefresh_BtnTapped(object sender, EventArgs e)
            => listView.BeginRefresh();
    }
}