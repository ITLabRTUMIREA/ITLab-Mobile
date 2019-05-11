using Http_Post.Controls;
using Http_Post.Services;
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
            this.IsEnabled = false;
            Title = Device.RuntimePlatform == Device.Android ? "" : Res.Resource.TitleNotifications;

            lblInfo.Text = Res.Resource.ErrorNoEvent;
            SetPadding();

            Subscribe();

            GetInvitations();
            if (GetRight())
                GetWishes();
            this.IsEnabled = true;
		}

        void SetPadding()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    stack.Padding = new Thickness(10, 30, 5, 5);
                    return;
                case Device.Android:
                    stack.Padding = new Thickness(10, 5, 5, 5);
                    return;
                case Device.UWP:
                    stack.Padding = new Thickness(10, 15, 5, 5);
                    return;
            }
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

                lblInfo.IsVisible = invitations.Data.Count() == 0 ? true : false;
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

        bool GetRight()
        {
            string whatToCheck = "CanEditEvent";
            foreach (var item in CurrentUserIdFactory.UserRoles)
                if (item.Equals(whatToCheck))
                    return true;
            return false;
        }

        async void GetWishes()
        {
            try
            {
                listWishes.IsVisible = true;
                var response = await client.GetStringAsync("event/wishers");
                var wishes = JsonConvert.DeserializeObject<ListResponse<WisherEventView>>(response);

                if (wishes.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {wishes.StatusCode}");

                lblInfo.IsVisible = wishes.Data.Count() == 0 ? true : false;
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
