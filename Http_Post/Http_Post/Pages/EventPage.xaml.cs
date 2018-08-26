using Http_Post.Classes;
using Http_Post.Extensions.Responses.Event;
using Http_Post.Services;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.Login;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using Xamarin.Forms;

namespace Http_Post.Pages
{
	public partial class EventPage : ContentPage
	{
        HttpClient client;

        private readonly string host = "labworkback.azurewebsites.net"; // labworkback.azurewebsites.net // localhost
        private readonly string port = "80"; // 80 // 5000

        private OneObjectResponse<LoginResponse> student;
        private ListResponse<CompactEventViewExtended> events;

        public EventPage(OneObjectResponse<LoginResponse> student)
        {
            InitializeComponent();
            this.student = student;
            Init();

            UpdateTheme();

            GetEvents();
        }

        private void Init()
        {
            Title = Res.Resource.Title_Event;
            client = HttpClientFactory.HttpClient;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", student.Data.AccessToken);
        }

        private void UpdateTheme()
        {
            var th = new ThemeChanger();
            var col = Application.Current.Resources;
            //col["themeStack"] = col[th.Theme + "_Stack"];
        }

        private async void GetEvents()
        {
            try
            {
                var response = await client.GetStringAsync($"http://{host}:{port}/api/event/");

                events = JsonConvert.DeserializeObject<ListResponse<CompactEventViewExtended>>(response);

                ShowEvents();
            }
            catch (Exception ex)
            {
                stacklayout.Children.Add(new Label { Text = ex.Message });
            }
        }

        private void ShowEvents()
        {
            try
            {
                listView.ItemsSource = events.Data;
            }
            catch (Exception ex)
            {
                stacklayout.Children.Add(new Label { Text = ex.Message });
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
    }
}