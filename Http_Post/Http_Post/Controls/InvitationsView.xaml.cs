using Http_Post.Res;
using Models.PublicAPI.Responses.Event.Invitations;
using Models.PublicAPI.Responses.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InvitationsView : ContentView
    {
        Guid placeId;
        Guid eventId;
        Action eventGood;
        Action eventBad;

        public InvitationsView (EventApplicationView invitation, Action eventGood, Action eventBad)
		{
            BindingContext = invitation;
			InitializeComponent ();

            placeId = invitation.PlaceId;
            eventId = invitation.Id;
            this.eventGood= eventGood;
            this.eventBad = eventBad;
            // TODO: localize
            lblPlaceDesc.Text = string.IsNullOrEmpty(invitation.PlaceDescription) ? Resource.ErrorNoDescription : invitation.PlaceDescription;
            lblTitle.Text = "Invitation";
            lblBegin.Text = "Begin:";
            lblDuration.Text = "Duration:";
            lblRole.Text = "Role:";
            lblPlace.Text = "Place:";
            btnAccept.Text = "Accept";
            btnReject.Text = "Reject";
		}

        async void btnAccept_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await Services.HttpClientFactory.HttpClient.PostAsync($"event/invitation/{placeId}/accept", null);

                var resultContent = await result.Content.ReadAsStringAsync();
                var message = JsonConvert.DeserializeObject<OneObjectResponse<EventApplicationView>>(resultContent);
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
                var result = await Services.HttpClientFactory.HttpClient.PostAsync($"event/invitation/{placeId}/reject", null);

                var resultContent = await result.Content.ReadAsStringAsync();
                var message = JsonConvert.DeserializeObject<OneObjectResponse<EventApplicationView>>(resultContent);
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