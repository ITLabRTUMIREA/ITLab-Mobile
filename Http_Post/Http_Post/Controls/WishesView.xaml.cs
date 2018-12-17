using Models.PublicAPI.Responses.Event.Invitations;
using Models.PublicAPI.Responses.General;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WishesView : ViewCell
	{
        WisherEventView wish;
        INavigation navigation;

        public WishesView (WisherEventView wish, INavigation navigation)
		{
            this.wish = wish;
            this.navigation = navigation;

            BindingContext = wish;
            InitializeComponent();

            // TODO: localize
            lblName.Text = $"{wish.User.FirstName} {wish.User.LastName}";
            lblRole.Text = "Role:";
            lblShift.Text = "Shift:";
            target.Text = $"Needs {wish.TargetParticipantsCount} participants. (now: {wish.CurrentParticipantsCount})";
            btnAccept.Text = "Accept";
            btnReject.Text = "Reject";

            lblTime.Text = wish.BeginTime.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
        }

        async void btnAccept_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await Services.HttpClientFactory.HttpClient.PostAsync($"event/wish/{wish.PlaceId}/{wish.User.Id}/accept", null);

                var resultContent = await result.Content.ReadAsStringAsync();
                var message = Newtonsoft.Json.JsonConvert.DeserializeObject<OneObjectResponse<WisherEventView>>(resultContent);
                if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {message.StatusCode}");

                MessagingCenter.Send<WisherEventView>(wish, "DeleteWish");
            }
            catch (Exception)
            {}
        }

        async void btnReject_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await Services.HttpClientFactory.HttpClient.PostAsync($"event/wish/{wish.PlaceId}/{wish.User.Id}/reject", null);

                var resultContent = await result.Content.ReadAsStringAsync();
                var message = Newtonsoft.Json.JsonConvert.DeserializeObject<OneObjectResponse<WisherEventView>>(resultContent);
                if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {message.StatusCode}");

                MessagingCenter.Send<WisherEventView>(wish, "DeleteWish");
            }
            catch (Exception)
            {}
        }

        void ViewCell_Tapped(object sender, EventArgs e)
            => navigation.PushAsync(new Pages.OneEventPage(wish.Id));
    }
}