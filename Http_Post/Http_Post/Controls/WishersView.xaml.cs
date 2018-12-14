using Models.PublicAPI.Responses.Event.Invitations;
using Models.PublicAPI.Responses.General;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WishersView : ContentView
	{
        Guid eventId;
        Guid placeId;
        Guid userId;
        Action eventGood;
        Action eventBad;

		public WishersView (WisherEventView wish, Action eventGood, Action eventBad)
		{
            BindingContext = wish;
			InitializeComponent ();

            eventId = wish.Id;
            placeId = wish.PlaceId;
            userId = wish.User.Id;
            this.eventGood = eventGood;
            this.eventBad = eventBad;
            // TODO: localize
            lblTitle.Text = "Wish";
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
                var result = await Services.HttpClientFactory.HttpClient.PostAsync($"event/wish/{placeId}/{userId}/accept", null);

                var resultContent = await result.Content.ReadAsStringAsync();
                var message = Newtonsoft.Json.JsonConvert.DeserializeObject<OneObjectResponse<WisherEventView>>(resultContent);
                if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {message.StatusCode}");

                eventGood?.Invoke();
            }
            catch (Exception ex)
            {
                eventBad?.Invoke();
            }
        }

        async void btnReject_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await Services.HttpClientFactory.HttpClient.PostAsync($"event/wish/{placeId}/{userId}/reject", null);

                var resultContent = await result.Content.ReadAsStringAsync();
                var message = Newtonsoft.Json.JsonConvert.DeserializeObject<OneObjectResponse<WisherEventView>>(resultContent);
                if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {message.StatusCode}");

                eventGood?.Invoke();
            }
            catch (Exception ex)
            {
                eventBad?.Invoke();
            }
        }

        async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
            => await Navigation.PushAsync(new Pages.OneEventPage(eventId));
    }
}