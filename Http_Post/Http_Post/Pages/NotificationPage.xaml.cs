using Http_Post.Controls;
using Models.PublicAPI.Responses.Event.Invitations;
using Models.PublicAPI.Responses.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        ListResponse<EventApplicationView> invitations;
        ListResponse<WisherEventView> wishers;

        Image pathCollapse = new Image
        {
            Source = "ArrowRight.png"
        };

        Image pathExpand = new Image
        {
            Source = "ArrowDown.png"
        };

        public NotificationPage ()
		{
			InitializeComponent ();

            Title = Device.RuntimePlatform == Device.UWP ? "Notifications" : "";
            lblInvitation.Text = "Invitations: ";
            lblWish.Text = "Wish: ";
            imageInvitation.Source = pathCollapse.Source;
            imageWish.Source = pathCollapse.Source;
            GetNotifications();
            GetWishers();
		}

        async void GetNotifications()
        {
            try
            {
                var response = await client.GetStringAsync("event/applications/invitations");
                invitations = JsonConvert.DeserializeObject<ListResponse<EventApplicationView>>(response);

                List<InvitationsView> invitationsViews = new List<InvitationsView>();
                foreach (var item in invitations.Data)
                    invitationsViews.Add(new InvitationsView(item));

                listInvitation.ItemsSource = invitationsViews;
                lblInvitation.Text = $"Invitations: {invitationsViews.Count}";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        async void GetWishers()
        {
            try
            {
                var response = await client.GetStringAsync("event/wishers");
                wishers = JsonConvert.DeserializeObject<ListResponse<WisherEventView>>(response);

                List<WishersView> wishersViews = new List<WishersView>();
                foreach (var item in wishers.Data)
                    wishersViews.Add(new WishersView(item));

                listWish.ItemsSource = wishersViews;
                lblWish.Text = $"Wish: {wishersViews.Count}";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        void Invitation_Tapped(object sender, EventArgs e)
        {
            if (imageInvitation.Source.Equals(pathCollapse.Source))
                imageInvitation.Source = pathExpand.Source;
            else
                imageInvitation.Source = pathCollapse.Source;
            listInvitation.IsVisible = !listInvitation.IsVisible;
        }

        void Wish_Tapped(object sender, EventArgs e)
        {
            if (imageWish.Source.Equals(pathCollapse.Source))
                imageWish.Source = pathExpand.Source;
            else
                imageWish.Source = pathCollapse.Source;
            listWish.IsVisible = !listWish.IsVisible;
        }

        async void listInvitation_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var tapped = (InvitationsView)e.Item;
            await Navigation.PushAsync(new OneEventPage(tapped.Id));
        }

        async void listWish_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var tapped = (WishersView)e.Item;
            await Navigation.PushAsync(new OneEventPage(tapped.Id));
        }
    }
}