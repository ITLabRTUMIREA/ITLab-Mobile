using Http_Post.Res;
using Models.PublicAPI.Responses.Event.Invitations;
using Models.PublicAPI.Responses.General;
using Newtonsoft.Json;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InvitationsView : ViewCell
    {
        EventApplicationView invitation;
        INavigation navigation;

        public InvitationsView (EventApplicationView invitation, INavigation navigation)
		{
            this.invitation = invitation;
            this.navigation = navigation;

            BindingContext = invitation;
			InitializeComponent ();

            // TODO: localize
            lblPlaceDesc.Text = string.IsNullOrEmpty(invitation.PlaceDescription) ? Resource.ErrorNoDescription : invitation.PlaceDescription;
            lblBegin.Text = "Begin:";
            lblDuration.Text = "Duration:";
            lblRole.Text = "Role:";
            lblPlace.Text = "Place:";
            btnAccept.Text = "Accept";
            btnReject.Text = "Reject";

            lblTime.Text = invitation.BeginTime.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
		}

        async void btnAccept_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await Services.HttpClientFactory.HttpClient.PostAsync($"event/invitation/{invitation.PlaceId}/accept", null);

                var resultContent = await result.Content.ReadAsStringAsync();
                var message = JsonConvert.DeserializeObject<OneObjectResponse<EventApplicationView>>(resultContent);
                if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {message.StatusCode}");

                MessagingCenter.Send<EventApplicationView>(invitation, "DeleteInvitation");
            }
            catch (Exception)
            { }
        }

        async void btnReject_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await Services.HttpClientFactory.HttpClient.PostAsync($"event/invitation/{invitation.PlaceId}/reject", null);

                var resultContent = await result.Content.ReadAsStringAsync();
                var message = JsonConvert.DeserializeObject<OneObjectResponse<EventApplicationView>>(resultContent);
                if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {message.StatusCode}");

                MessagingCenter.Send<EventApplicationView>(invitation, "DeleteInvitation");
            }
            catch (Exception)
            { }
        }

        void ViewCell_Tapped(object sender, EventArgs e)
            => navigation.PushAsync(new Pages.OneEventPage(invitation.Id));
    }
}