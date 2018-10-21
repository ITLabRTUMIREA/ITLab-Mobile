using Http_Post.Classes;
using Http_Post.Extensions.Responses.Event;
using Http_Post.Services;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using Xamarin.Forms;

namespace Http_Post.Pages
{
	public partial class EventPage : ContentPage
	{
        HttpClient client = HttpClientFactory.HttpClient;

        private ListResponse<CompactEventViewExtended> events;

        public EventPage()
        {
            InitializeComponent();
            Title = Res.Resource.TitleEvents;

            UpdateTheme();

            GetEvents();
        }

        private void UpdateTheme()
        {
            var th = new ThemeChanger().Theme;
            var col = Application.Current.Resources;
            col["themeStack"] = col[th + "_Stack"];
        }

        private async void GetEvents()
        {
            try
            {
                var response = await client.GetStringAsync("event/");

                events = JsonConvert.DeserializeObject<ListResponse<CompactEventViewExtended>>(response);

                ShowEvents();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private async void ShowEvents()
        {
            try
            {
                listView.ItemsSource = events.Data;
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
            
                Navigation.PushAsync(new OneEventPage(item.Id, item.BeginTime, item.EndTime));
            }
            catch (Exception ex)
            {
                stacklayout.Children.Add(new Label { Text = ex.Message });
            }
        }
    }
}