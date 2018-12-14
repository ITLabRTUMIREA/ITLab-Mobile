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

        public NotificationPage ()
		{
			InitializeComponent ();
            Title = Device.RuntimePlatform == Device.UWP ? "Notifications" : "";
            GetAll();
		}

        void GetAll()
        {
            for (int i = 2; i < stackLayout.Children.Count; i++)
                stackLayout.Children.RemoveAt(i);
            GetNotifications();
            GetWishers();
        }

        async void ShowError()
            => await DisplayAlert("Error", "Что-то пошло не так", "Ok");


        async void GetNotifications()
        {
            try
            {
                var response = await client.GetStringAsync("event/applications/invitations");
                invitations = JsonConvert.DeserializeObject<ListResponse<EventApplicationView>>(response);

                foreach (var invite in invitations.Data)
                    stackLayout.Children.Add(new Controls.InvitationsView(invite, GetAll, ShowError));
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

                foreach (var wish in wishers.Data)
                    stackLayout.Children.Add(new Controls.WishersView(wish, GetAll, ShowError));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        void Button_Tapped(object sender, EventArgs e)
        {
            GetAll();
        }
    }
}