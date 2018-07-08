using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.Login;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;

using Xamarin.Forms;

namespace Http_Post.Pages
{
	public partial class EventPage : ContentPage
	{
        HttpClient client;

        private string host = "labworkback.azurewebsites.net";
        private string port = "80";

        private OneObjectResponse<LoginResponse> student;

        public EventPage ()
		{
			InitializeComponent ();
		}

        public EventPage(OneObjectResponse<LoginResponse> student)
        {
            InitializeComponent();

            this.student = student;
            GetEvents();
        }

        private async void GetEvents()
        {
            try
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", student.Data.Token);

                var response = await client.GetStringAsync($"http://{host}:{port}/api/event/");
                var events = JsonConvert.DeserializeObject<ListResponse<EventPresent>>(response);

                listView.ItemsSource = events.Data.Select(ev => ev.Title ?? "no title");
            }
            catch (Exception ex)
            {
                stacklayout.Children.Add(new Label { Text = ex.Message });
            }
        }
    }
}