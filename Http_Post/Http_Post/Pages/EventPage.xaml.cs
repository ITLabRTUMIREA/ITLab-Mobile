using Http_Post.Extensions.Responses.Event;
using Http_Post.Services;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventPage : ContentPage
    {
        HttpClient client = HttpClientFactory.HttpClient;

        private ListResponse<CompactEventViewExtended> eventsAll;
        private ListResponse<CompactEventViewExtended> eventsToday;
        bool _All;

        public EventPage()
        {
            _All = false;
            InitializeComponent();
            this.IsEnabled = false;
            Title = Device.RuntimePlatform == Device.Android ? "" : Res.Resource.TitleEvents;
            lblFooter.Text = Res.Resource.ADMIN_Updated + ": " + DateTime.Now.ToString("f");

            listView.Refreshing += (s, e) => {
                this.IsEnabled = false;
                GetEvents();
                lblFooter.Text = Res.Resource.ADMIN_Updated + ": " + DateTime.Now.ToString("f");
                listView.IsRefreshing = false;
                this.IsEnabled = true;
            };
            GetEvents();
            btnCreate.IsVisible = GetRight();
            this.IsEnabled = true;
        }

        async void GetEvents()
        {
            try
            {
                var response = await client.GetStringAsync($"event/?begin={DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")}");
                eventsToday = JsonConvert.DeserializeObject<ListResponse<CompactEventViewExtended>>(response);
                lblHeader.Text = $"{Res.Resource.ActualEvents}: {eventsToday.Data.Count().ToString()}";

                response = await client.GetStringAsync("event/");
                eventsAll = JsonConvert.DeserializeObject<ListResponse<CompactEventViewExtended>>(response);
                listView.ItemsSource = _All ? 
                    eventsAll.Data.Reverse() : eventsToday.Data.Reverse();
                btnShowEvents.ImageSource = _All ? "News" : "Today";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
                this.IsEnabled = true;
            }
        }

        async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = (CompactEventView)e.Item;
            await Navigation.PushAsync(new OneEventPage(item.Id));
        }

        bool GetRight()
        {
            string whatToCheck = "CanEditEvent";
            foreach (var item in CurrentUserIdFactory.UserRoles)
                if (item.Equals(whatToCheck))
                    return true;
            return false;
        }

        void btnShowEvents_Clicked(object sender, EventArgs e)
        {
            _All = !_All;
            btnShowEvents.ImageSource = _All ? "News" : "Today";
            listView.ItemsSource = _All ? eventsAll.Data.Reverse() : eventsToday.Data.Reverse();
        }

        void btnMyEvents_Clicked(object sender, EventArgs e)
        {
            listView.ItemsSource = eventsAll.Data.Where(ev => ev.Participating).Reverse();
            _All = !_All;
        }

        async void btnCreate_Clicked(object sender, EventArgs e)
            => await Navigation.PushAsync(new CreateEventPage());

        void btnRefresh_BtnTapped(object sender, EventArgs e)
            => listView.BeginRefresh();
    }
}