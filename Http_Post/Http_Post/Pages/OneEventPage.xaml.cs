using Http_Post.Extensions.Responses.Event;
using Http_Post.Services;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using Xamarin.Forms;

namespace Http_Post.Pages
{
    public partial class OneEventPage : ContentPage
    {
        //TODO: btnChange -> push to CreateEvent like CreateEquipment
        private readonly HttpClient client = HttpClientFactory.HttpClient;
        private readonly Guid eventId;
        private EventViewExtended OneEvent { get; set; }

        private DateTime beginTime;
        private DateTime endTime;

        public OneEventPage(Guid id, DateTime begin, DateTime end)
        {
            eventId = id;
            beginTime = begin;
            endTime = end;
            Show();
        }

        private void Init()
        {
            BindingContext = OneEvent; // necessarilly before 'InitializeComponent'
            InitializeComponent();

            listView.ItemsSource = OneEvent.Shifts;
            UpdateLanguage();
        }

        private async void Show()
        {
            try
            {
                var response = await client.GetStringAsync($"event/{eventId}");
                var receivedData = JsonConvert.DeserializeObject<OneObjectResponse<EventViewExtended>>(response);

                if (receivedData.StatusCode != ResponseStatusCode.OK)
                    throw new Exception($"error {receivedData.StatusCode}");
                OneEvent = receivedData.Data;

                Init(); // init binding content 

            } catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // push to shift page
            Navigation.PushAsync(new OneShiftViewPage(e.Item as ShiftView));
        }

        private void UpdateLanguage()
        {
            lblShifts.Text = Res.Resource.Shifts;
            lblBeginTime.Text = beginTime.ToLocalTime().ToString("dd MMMM, yyyy. HH:mm");
            lblEndTime.Text = endTime.ToLocalTime().ToString("dd MMMM, yyyy. HH:mm");
            btnChange.Text = Res.Resource.Change;
        }

        private void btnChange_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CreateEventPage(OneEvent));
        }
    }
}