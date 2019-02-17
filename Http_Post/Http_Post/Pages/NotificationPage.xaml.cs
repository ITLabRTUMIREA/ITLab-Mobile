using Http_Post.Controls;
using Models.PublicAPI.Responses.Event.Invitations;
using Models.PublicAPI.Responses.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NotificationPage : ContentPage
	{
        HttpClient client = Services.HttpClientFactory.HttpClient;

        public NotificationPage ()
		{
			InitializeComponent ();

            Title = Device.RuntimePlatform == Device.UWP ? Res.Resource.TitleNotifications : "";
            lblInvitations.Text = $"{Res.Resource.Invitations}: {Res.Resource.ErrorNoData}";
            lblWishes.Text = $"{Res.Resource.Wishes}: {Res.Resource.ErrorNoData}";

            Subscribe();

            GetInvitations();
            GetWishes();
		}

        void Subscribe()
        {
            MessagingCenter.Subscribe<EventApplicationView>(
                this,
                "DeleteInvitation",
                (sender) =>
                {
                    GetInvitations();
                    GetWishes();
                });
            MessagingCenter.Subscribe<WisherEventView>(
                this,
                "DeleteWish",
                (sender) =>
                {
                    GetInvitations();
                    GetWishes();
                });
        }

        async void GetInvitations()
        {
            try
            {
                var response = await client.GetStringAsync("event/applications/invitations");
                var invitations = JsonConvert.DeserializeObject<ListResponse<EventApplicationView>>(response);
                
                if (invitations.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {invitations.StatusCode}");

                lblInvitations.Text = $"{Res.Resource.Invitations}: {invitations.Data.Count()}";
                listInvitations.ItemsSource = invitations.Data;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        void listInvitations_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var tapped = e.Item as EventApplicationView;
            Navigation.PushAsync(new Pages.OneEventPage(tapped.Id));
        }

        async void GetWishes()
        {
            try
            {
                var response = await client.GetStringAsync("event/wishers");
                var wishes = JsonConvert.DeserializeObject<ListResponse<WisherEventView>>(response);

                if (wishes.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {wishes.StatusCode}");

                lblWishes.Text = $"{Res.Resource.Wishes}: {wishes.Data.Count()}";
                listWishes.ItemsSource = wishes.Data;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        void listWishes_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var tapped = e.Item as WisherEventView;
            Navigation.PushAsync(new Pages.OneEventPage(tapped.Id));
        }
    }
}