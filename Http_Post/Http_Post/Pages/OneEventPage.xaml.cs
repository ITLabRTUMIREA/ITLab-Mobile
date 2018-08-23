using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses;
using Newtonsoft.Json;
using System;
using System.Net.Http;

using Xamarin.Forms;
using Models.PublicAPI.Responses.Login;
using Http_Post.Extensions.Responses.Event;
using Models.PublicAPI.Responses.Event;

namespace Http_Post.Pages
{
    public partial class OneEventPage : ContentPage
    {
        private readonly string host = "labworkback.azurewebsites.net"; // labworkback.azurewebsites.net // localhost
        private readonly string port = "80"; // 80 // 5000

        private HttpClient client;
        private readonly Guid eventId;
        private EventViewExtended OneEvent { get; set; }

        public OneEventPage(Guid id, HttpClient httpClient)
        {
            client = httpClient;
            eventId = id;
            Show();
        }

        private void Init()
        {
            BindingContext = OneEvent; // necessarilly before 'InitializeComponent'
            InitializeComponent();
            listView.ItemsSource = OneEvent.Shifts;
            lbl.Text = Res.Resource.Shifts;
            UpdateTheme();
        }

        private async void Show()
        {
            try
            {
                var response = await client.GetStringAsync($"http://{host}:{port}/api/event/{eventId}");
                var receivedData = JsonConvert.DeserializeObject<OneObjectResponse<EventViewExtended>>(response);

                if (receivedData.StatusCode != ResponseStatusCode.OK)
                    throw new Exception($"error {receivedData.StatusCode}");
                OneEvent = receivedData.Data;

                Init(); // init binding content 

            } catch (Exception ex)
            {
                stacklayout.Children.Add(new Label { Text = ex.Message });
            }
        }

        private void UpdateTheme()
        {
            
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var tapped = e.Item as ShiftView;
            var index = OneEvent.Shifts.IndexOf(tapped);
            var a = 8;
        }
    }
}