using Http_Post.Classes;
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

        private ListResponse<CompactEventViewExtended> events;

        public EventPage()
        {
            BindingContext = ThemeChanger.Theme;
            InitializeComponent();
            Title = Device.RuntimePlatform == Device.UWP ? Res.Resource.TitleEvents : "";

            listView.Refreshing += (s, e) => {
                GetEvents();
                listView.IsRefreshing = false;
            };
            GetEvents();
            ChangeToolBar();
        }

        async void GetEvents()
        {
            try
            {
                var response = await client.GetStringAsync("event/");

                events = JsonConvert.DeserializeObject<ListResponse<CompactEventViewExtended>>(response);

                listView.ItemsSource = events.Data.Reverse();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        public void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            { 
                var item = (CompactEventView) e.Item;
            
                Navigation.PushAsync(new OneEventPage(item.Id));
            }
            catch (Exception ex)
            {
                stacklayout.Children.Add(new Label { Text = ex.Message });
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
    }
}