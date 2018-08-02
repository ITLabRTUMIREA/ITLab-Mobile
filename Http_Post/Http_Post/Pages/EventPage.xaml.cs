using Http_Post.Classes;
using Http_Post.Extensions.Responses.Event;
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

        public EventPage ()
		{
			InitializeComponent ();
        }

        public EventPage(OneObjectResponse<LoginResponse> student)
        {
            InitializeComponent();
            Title = Res.Resource.Title_Event;

            this.student = student;
            GetEvents();

            UpdateTheme();
        }

        private void UpdateTheme()
        {
            ThemeChanger themeChanger = new ThemeChanger();
            stacklayout.BackgroundColor = themeChanger.ColorBG();
            listView.BackgroundColor = themeChanger.ColorBG();
        }

        private async void GetEvents()
        {
            try
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", student.Data.Token);

                // Phone localhost debug http://ce8604c9.ngrok.io
                //var response = await client.GetStringAsync($"http://ce8604c9.ngrok.io/api/event/");

                var response = await client.GetStringAsync($"http://{host}:{port}/api/event/");

                events = JsonConvert.DeserializeObject<ListResponse<CompactEventViewExtended>>(response);

                ShowEvents();
            }
            catch (Exception ex)
            {
                stacklayout.Children.Add(new Label { Text = ex.Message });
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
                stacklayout.Children.Add(new Label { Text = ex.Message });
            }
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (CompactEventView)e.SelectedItem;

            // Open new page
        }
    }
}