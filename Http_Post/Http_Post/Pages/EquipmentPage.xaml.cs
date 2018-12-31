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
        FFImageLoading.Svg.Forms.SvgCachedImage imgUp = new FFImageLoading.Svg.Forms.SvgCachedImage()
        {
            Source = Images.ImageManager.GetSourceImage("ArrowUp")
        };
        FFImageLoading.Svg.Forms.SvgCachedImage imgDown = new FFImageLoading.Svg.Forms.SvgCachedImage()
        {
            Source = Images.ImageManager.GetSourceImage("ArrowDown")
        };

        public EquipmentPage ()
		{
            Init();
            GetEquipment();
            btnCreate.IsVisible = GetRight();

            listView.Refreshing += (s, e) => {
                GetEquipment();
                listView.IsRefreshing = false;
            };
        }

        void Init()
        {
            InitializeComponent();
            UpdateLanguage();
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
                btnType.FontAttributes = FontAttributes.Bold;
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
            btnType.Text = Resource.EquipmentType;
            btnOwner.Text = Resource.Owner;
            btnNumber.Text = Resource.Number;
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
        void btnType_Clicked(object sender, EventArgs e)
        {
            if (!reserve && btnType.FontAttributes == FontAttributes.Bold)
            {
                listEquip = listEquip.OrderByDescending(se => se.EquipmentType.Title);
                imageType.Source = imgUp.Source;
            }
            else
            {
                listEquip = listEquip.OrderBy(se => se.EquipmentType.Title);
                imageType.Source = imgDown.Source;
                imageOwner.Source = imgDown.Source;
                imageNumber.Source = imgDown.Source;
            }

            reserve = btnType.FontAttributes == FontAttributes.Bold ? !reserve : false;
            listView.ItemsSource = listEquip; 
            btnType.FontAttributes = FontAttributes.Bold;
            btnOwner.FontAttributes = FontAttributes.None;
            btnNumber.FontAttributes = FontAttributes.None;
        }

        void btnOwner_Clicked(object sender, EventArgs e)
        {
            if (!reserve && btnOwner.FontAttributes == FontAttributes.Bold)
            {
                listEquip = listEquip.OrderByDescending(se => se.OwnerName);
                imageOwner.Source = imgUp.Source;
            }
            else
            {
                listEquip = listEquip.OrderBy(se => se.OwnerName);
                imageOwner.Source = imgDown.Source;
                imageType.Source = imgDown.Source;
                imageNumber.Source = imgDown.Source;
            }

            reserve = btnOwner.FontAttributes == FontAttributes.Bold ? !reserve : false;
            listView.ItemsSource = listEquip;
            btnType.FontAttributes = FontAttributes.None;
            btnOwner.FontAttributes = FontAttributes.Bold;
            btnNumber.FontAttributes = FontAttributes.None;
        }

        void btnNumber_Clicked(object sender, EventArgs e)
        {
            if (!reserve && btnNumber.FontAttributes == FontAttributes.Bold)
            {
                listEquip = listEquip.OrderByDescending(se => se.Number);
                imageNumber.Source = imgUp.Source;
            }
            else
            {
                listEquip = listEquip.OrderBy(se => se.Number);
                imageNumber.Source = imgDown.Source;
                imageOwner.Source = imgDown.Source;
                imageType.Source = imgDown.Source;
            }

            reserve = btnNumber.FontAttributes == FontAttributes.Bold ? !reserve : false;
            listView.ItemsSource = listEquip;
            btnType.FontAttributes = FontAttributes.None;
            btnOwner.FontAttributes = FontAttributes.None;
            btnNumber.FontAttributes = FontAttributes.Bold;
        }

        async void btnCreate_Clicked(object sender, EventArgs e)
            => await Navigation.PushAsync(new CreateEquipment());
    }
}