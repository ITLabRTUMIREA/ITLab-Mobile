using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Models.PublicAPI.Responses.Login;

namespace Http_Post.Pages
{
    public partial class OneEventPage : ContentPage
    {
        private readonly string host = "labworkback.azurewebsites.net"; // labworkback.azurewebsites.net // localhost
        private readonly string port = "80"; // 80 // 5000

        private HttpClient client;
        private readonly OneObjectResponse<LoginResponse> student;
        private readonly Guid eventId;
        public EventView Event { get; set; }

        public OneEventPage()
        {
            InitializeComponent();
        }

        public OneEventPage(Guid id, OneObjectResponse<LoginResponse> student)
        {
            this.student = student;
            eventId = id;
            Show();
        }

        private void Init()
        {
            BindingContext = Event;
            InitializeComponent();
            UpdateTheme();
        }

        private async void Show()
        {
            try
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", student.Data.Token);

                var response = await client.GetStringAsync($" http://{host}:{port}/api/event/{eventId}");
                var receivedData = JsonConvert.DeserializeObject<OneObjectResponse<EventView>>(response);

                if (receivedData.StatusCode != ResponseStatusCode.OK)
                    throw new Exception($"error {receivedData.StatusCode}");
                Event = receivedData.Data;

                Init();
            } catch (Exception ex)
            {
                stacklayout.Children.Add(new Label { Text = ex.Message });
            }
        }

        private void UpdateTheme()
        {
            
        }
    }
}