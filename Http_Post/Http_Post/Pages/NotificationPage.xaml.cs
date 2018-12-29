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
        ObservableCollection<EventApplicationView> invitations;
        ObservableCollection<WisherEventView> wishes;

        public NotificationPage ()
		{
			InitializeComponent ();

            Title = Device.RuntimePlatform == Device.UWP ? Res.Resource.TitleNotifications : "";
            //Icon = Images.ImageManager.GetSourceImage("Notifications");
            tableInvitations.Title = $"{Res.Resource.Invitations}: {Res.Resource.ErrorNoData}";
            tableWishes.Title = $"{Res.Resource.Wishes}: {Res.Resource.ErrorNoData}";

            invitations = new ObservableCollection<EventApplicationView>();
            wishes = new ObservableCollection<WisherEventView>();

            GetNotifications();
            GetWishes();

            Subscribe();
		}

        void Subscribe()
        {
            MessagingCenter.Subscribe<EventApplicationView>(
                this,
                "DeleteInvitation",
                (sender) =>
                {
                    GetNotifications();
                    GetWishes();
                });
            MessagingCenter.Subscribe<WisherEventView>(
                this,
                "DeleteWish",
                (sender) =>
                {
                    GetNotifications();
                    GetWishes();
                });
        }

        async void GetNotifications()
        {
            try
            {
                this.invitations.Clear();
                tableInvitations.Clear();
                var response = await client.GetStringAsync("event/applications/invitations");
                var invitations = JsonConvert.DeserializeObject<ListResponse<EventApplicationView>>(response);
                
                if (invitations.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {invitations.StatusCode}");

                foreach(var invitation in invitations.Data)
                {
                    this.invitations.Add(invitation);
                    tableInvitations.Add(new InvitationsView(invitation, Navigation));
                }

                tableInvitations.Title = $"{Res.Resource.Invitations}: {this.invitations.Count}";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        async void GetWishes()
        {
            try
            {
                this.wishes.Clear();
                tableWishes.Clear();
                var response = await client.GetStringAsync("event/wishers");
                var wishes = JsonConvert.DeserializeObject<ListResponse<WisherEventView>>(response);

                if (wishes.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {wishes.StatusCode}");

                foreach (var wish in wishes.Data)
                {
                    this.wishes.Add(wish);
                    tableWishes.Add(new WishesView(wish, Navigation));
                }
                tableWishes.Title = $"{Res.Resource.Wishes}: {this.wishes.Count}";
            }
            catch (Exception ex)
            {
               await DisplayAlert("Error", ex.Message, "Ok");
            }
        }
    }
}