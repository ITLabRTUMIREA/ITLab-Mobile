using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.Login;
using Xamarin.Forms;
using Http_Post.Extensions.Responses.Event;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.Event;

namespace Http_Post.Pages
{
	public partial class DateSearchPage : ContentPage
	{
        private HttpClient client;
        private OneObjectResponse<LoginResponse> student;
        private readonly string host = "labworkback.azurewebsites.net"; // labworkback.azurewebsites.net // localhost
        private readonly string port = "80"; // 80 // 5000

        public DateSearchPage(OneObjectResponse<LoginResponse> student)
        {
			InitializeComponent ();
            SetDates();
            this.student = student;
            UpdateTheme();
        }

        private void SetDates()
        {
            DateEnd.Date = DateTime.Now.AddDays(3);
            DateBegin.MinimumDate = DateTime.Now.AddDays(-7);
            DateEnd.MaximumDate = DateTime.Now.AddDays(31);
        }

        private async void BtnSearch_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (DateEnd.Date < DateBegin.Date)
                    throw new Exception($"Error: {Res.Resource.DateError}"); // Ending date can't be less than begining date!

                client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", student.Data.AccessToken);

                var begin = DateBegin.Date.ToString("yyyy-MM-ddTHH:mm:ss");
                var end = DateEnd.Date.ToString("yyyy-MM-ddTHH:mm:ss");
                var response = await client.GetStringAsync($"http://{host}:{port}/api/event/?begin={begin}&end={end}");

                var events = JsonConvert.DeserializeObject<ListResponse<CompactEventViewExtended>>(response);
                if (events.StatusCode != ResponseStatusCode.OK)
                    throw new Exception($"Error: {events.StatusCode}");

                if (events.Data.Count() == 0)
                    throw new Exception($"Error: {Res.Resource.DateErrorNoEvents}");
                listView.ItemsSource = events.Data;
            }
            catch (Exception ex)
            {
                errorLabel.Text = ex.Message;
            }
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                var item = (CompactEventView)e.Item;

                Navigation.PushAsync(new OneEventPage(item.Id, client));
            }
            catch (Exception ex)
            {
                errorLabel.Text = ex.Message;
            }
        }

        private void UpdateTheme()
        {
            errorLabel.Text = Res.Resource.DateSearching;
            BtnSearch.Text = Res.Resource.DateSearchingBtn;
        }
    }
}