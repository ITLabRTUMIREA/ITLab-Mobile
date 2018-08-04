using Models.PublicAPI.Responses.Event;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Http_Post.Pages
{
	public partial class OneEventPage : ContentPage
	{
        private readonly string host = "labworkback.azurewebsites.net"; // labworkback.azurewebsites.net // localhost
        private readonly string port = "80"; // 80 // 5000
        private readonly Guid eventId;

        public OneEventPage ()
		{
			InitializeComponent ();
		}

        public OneEventPage(Guid id)
        {
            InitializeComponent();

            eventId = id;
            UpdateTheme();
            Show();
        }

        private async void Show()
        {
            try
            {
                var response = await new HttpClient().GetStringAsync($" http://{host}:{port}/api/event/{eventId}");
                var oneEvent = JsonConvert.DeserializeObject<EventView>(response);

            } catch (Exception ex)
            {

            }
        }

        private void UpdateTheme()
        {
            
        }
    }
}