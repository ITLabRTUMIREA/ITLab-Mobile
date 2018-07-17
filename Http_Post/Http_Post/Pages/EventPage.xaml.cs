using Http_Post.Controls;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.Login;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private ListResponse<CompactEventView> events;

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
        }

        private async void GetEvents()
        {
            try
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", student.Data.Token);

                var response = await client.GetStringAsync($"http://{host}:{port}/api/event/");
                events = JsonConvert.DeserializeObject<ListResponse<CompactEventView>>(response);

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
    }

    public class CompactEventView
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public EventTypePresent EventType { get; set; }
        public int Сompleteness { get; set; }
        public DateTime BeginTime { get; set; }
        public double TotalDurationInMinutes { get; set; }
        public int ShiftsCount { get; set; }
        public double CustomProg => Сompleteness / 100.0;
    }
}