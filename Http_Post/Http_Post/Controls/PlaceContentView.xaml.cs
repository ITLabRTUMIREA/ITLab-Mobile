using Models.PublicAPI.Responses.Event;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlaceContentView : ContentView
	{
        Image pathCollapse = new Image
        {
            Source = "ArrowRight.png"
        };
        Image pathExpand = new Image
        {
            Source = "ArrowDown.png"
        };
        PlaceView place;

        public PlaceContentView (PlaceView placeView, int numberOfPlace)
		{
            place = placeView;
			InitializeComponent ();

            int total = placeView.Participants.Count + placeView.Wishers.Count + placeView.Invited.Count + placeView.Unknowns.Count;
            lblPlaceNumber.Text = $"#{numberOfPlace} | {Res.Resource.Participants}: {total} {Res.Resource.Of} {placeView.TargetParticipantsCount}";
            lblDescription.Text = string.IsNullOrEmpty(placeView.Description) ? Res.Resource.ErrorNoDescription : placeView.Description;
            image.Source = pathCollapse.Source;
            btnWish.Text = Res.Resource.SendWish;

            AddPeople();
            AddEquipment();
        }

        void PlaceNumber_Tapped(object sender, EventArgs e)
        {
            if (image.Source.Equals(pathCollapse.Source))
                image.Source = pathExpand.Source;
            else
                image.Source = pathCollapse.Source;

            stackToHide.IsVisible = !stackToHide.IsVisible;
        }

        void AddPeople()
        {
            stackPart.Children.Clear();
            foreach (var part in place.Participants)
            {
                var lblName = new Label { Text = part.User.FirstName + " " + part.User.LastName + ", " + part.EventRole.Title, FontAttributes = FontAttributes.Bold };
                var lblParticipant = new Label { Text = Res.Resource.Participant, FontSize = lblName.FontSize - 2, FontAttributes = FontAttributes.Italic };
                var lblMail= new Label { Text = part.User.Email, FontSize = lblName.FontSize - 4 };
                stackPart.Children.Add(lblName);
                stackPart.Children.Add(lblParticipant);
                stackPart.Children.Add(lblMail);
            }

            foreach (var inv in place.Invited)
            {
                var lblName = new Label { Text = inv.User.FirstName + " " + inv.User.LastName + ", " + inv.EventRole.Title, FontAttributes = FontAttributes.Bold };
                var lblInvited = new Label { Text = Res.Resource.Invited, FontSize = lblName.FontSize - 2, FontAttributes = FontAttributes.Italic };
                var lblMail = new Label { Text = inv.User.Email, FontSize = lblName.FontSize - 4 };
                stackPart.Children.Add(lblName);
                stackPart.Children.Add(lblInvited);
                stackPart.Children.Add(lblMail);
            }

            foreach (var wishers in place.Wishers)
            {
                var lblName = new Label { Text = wishers.User.FirstName + " " + wishers.User.LastName + ", " + wishers.EventRole.Title, FontAttributes = FontAttributes.Bold };
                var lblWisher = new Label { Text = Res.Resource.Wisher, FontSize = lblName.FontSize - 2, FontAttributes = FontAttributes.Italic };
                var lblMail = new Label { Text = wishers.User.Email, FontSize = lblName.FontSize - 4 };
                stackPart.Children.Add(lblName);
                stackPart.Children.Add(lblWisher);
                stackPart.Children.Add(lblMail);
            }

            foreach (var unknowns in place.Unknowns)
            {
                var lblName = new Label { Text = unknowns.User.FirstName + " " + unknowns.User.LastName + ", " + unknowns.EventRole.Title, FontAttributes = FontAttributes.Bold };
                var lblUnknown = new Label { Text = "UNKNOWN", FontSize = lblName.FontSize - 2, FontAttributes = FontAttributes.Italic };
                var lblMail = new Label { Text = unknowns.User.Email, FontSize = lblName.FontSize - 4 };
                stackPart.Children.Add(lblName);
                stackPart.Children.Add(lblUnknown);
                stackPart.Children.Add(lblMail);
            }

            stackPart.IsVisible = stackPart.Children.Count == 0 ? false : true;
        }

        void AddEquipment()
        {
            foreach(var equip in place.Equipment)
            {
                var lblType = new Label { Text = equip.EquipmentType.Title, FontAttributes = FontAttributes.Bold };
                var lblSerial = new Label { Text = equip.SerialNumber, FontSize = lblType.FontSize - 4 };
                stackEquip.Children.Add(lblType);
                stackEquip.Children.Add(lblSerial);
            }

            stackEquip.IsVisible = stackEquip.Children.Count == 0 ? false : true;
        }

        async void btnWish_Clicked(object sender, EventArgs e)
        {
            try
            {
                var roleId = await new Popup.Event.WishTo().GetRoleIdAsync(Navigation);
                if (roleId.Equals(Guid.Empty))
                    return;

                var request = await Services.HttpClientFactory.HttpClient.PostAsync($"event/wish/{place.Id}/{roleId}", null);
                var requestContent = await request.Content.ReadAsStringAsync();
                var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.PublicAPI.Responses.ResponseBase>(requestContent);
                if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {message.StatusCode}");

                btnWish.Text = Res.Resource.YouSentWish;
            }
            catch (Exception)
            {
                btnWish.Text = "Error";
            }
        }
    }
}