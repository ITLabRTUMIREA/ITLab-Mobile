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
        bool clicked;

        public EventPage()
        {
            _All = clicked = false;
            InitializeComponent();
            Title = Device.RuntimePlatform == Device.UWP ? Res.Resource.TitleEvents : "";
            btnShowEvents.Text = "Show all events";
            lblFooter.Text = Res.Resource.ADMIN_Updated + ": " + DateTime.Now.ToString("f");

            listView.Refreshing += (s, e) => {
                GetEvents();
                lblFooter.Text = Res.Resource.ADMIN_Updated + ": " + DateTime.Now.ToString("f");
                listView.IsRefreshing = false;
            };
            GetEvents();
            ChangeToolBar();
        }

        async void GetEvents()
        {
            try
            {
                var response = await client.GetStringAsync($"event/?begin={DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")}");
                eventsToday = JsonConvert.DeserializeObject<ListResponse<CompactEventViewExtended>>(response);

                response = await client.GetStringAsync("event/");
                eventsAll = JsonConvert.DeserializeObject<ListResponse<CompactEventViewExtended>>(response);

                listView.ItemsSource = eventsToday.Data.Reverse();
                btnShowEvents.IsVisible = true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
                clicked = false;
            }
        }

        async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                var item = (CompactEventView)e.Item;

                await Navigation.PushAsync(new OneEventPage(item.Id));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
                clicked = false;
            }
        }

        void ChangeToolBar()
        {
            if (!GetRight())
                return;

            var itemChange = new ToolBar.ToolBarItems().Item(null, 1, ToolbarItemOrder.Primary, "CreateCircle.png");
            itemChange.Clicked += async (s, e) =>
            {
                await Navigation.PushAsync(new CreateEventPage());
            };

            ToolbarItems.Add(itemChange);
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
            btnShowEvents.Text = _All ? "Show for today" : "Show all events";
            listView.ItemsSource = _All ? eventsAll.Data.Reverse() : eventsToday.Data.Reverse();
        }
    }
}