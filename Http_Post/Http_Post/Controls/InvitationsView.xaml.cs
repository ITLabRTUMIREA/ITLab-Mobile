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

        public InvitationsView()
        {
            InitializeComponent();
            BindingContextChanged += InvitationsView_BindingContextChanged;

            lblDuration.Text = $"{Resource.Duration}:";
            lblPlace.Text = $"{Resource.Place}:";
            btnAccept.Text = Resource.Accept;
            btnReject.Text = Resource.Reject;
        }

        void InvitationsView_BindingContextChanged(object sender, EventArgs e)
        {
            invitation = BindingContext as EventApplicationView;
            if (invitation == null)
                return;
            lblPlaceDesc.Text = string.IsNullOrEmpty(invitation?.PlaceDescription) ? Resource.ErrorNoDescription : invitation.PlaceDescription;
            lblTime.Text = invitation?.BeginTime.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
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
    }
}