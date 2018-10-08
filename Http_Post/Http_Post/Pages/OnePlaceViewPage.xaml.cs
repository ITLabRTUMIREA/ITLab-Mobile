using Http_Post.Res;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;
using Newtonsoft.Json;

using Xamarin.Forms;

namespace Http_Post.Pages
{
	public partial class OnePlaceViewPage : ContentPage
	{
        PlaceView place;
		public OnePlaceViewPage (PlaceView place, int index)
		{
			InitializeComponent ();
            Title = $"{Resource.Place} №" + index;
            this.place = place;

            UpdateLanguage();
            SetProperties();
		}

        void UpdateLanguage()
        {
            noEquip.Text = Resource.NoEquipmentError; // if no equipment in this place
            noPart.Text = Resource.NoParticipantsError; // if no participants in this place
            lblParticipants.Text = $"{Resource.Participants}:"; // set title
            lblEquipment.Text = $"{Resource.Equipment}:"; // set title
            lblDescription.Text = string.IsNullOrEmpty(place.Description) ? Resource.NoDescriptionError : place.Description; // set description
            lblTarget.Text = $"{Resource.Participants}: " + place.Participants.Count.ToString() + $" {Resource.Of} " + place.TargetParticipantsCount.ToString(); // set how many participants now in this place
        }

        // Set properties for better view
        async void SetProperties()
        {
            #region set participants
            for (int i = 0; i < place.Participants.Count; i++)
            {
                stackParticipants.Children.Clear();
                stackParticipants.Children.Add(new Label
                {
                    Text = place.Participants[i].User.FirstName +
                            place.Participants[i].User.LastName + '(' +
                            place.Participants[i].EventRole.Title + ')',
                    Style = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Lbl"] as Style,
                    TextColor = Color.Green,
                });
            }
            #endregion
            #region set people who were invited
            for (int i = 0; i < place.Invited.Count; i++)
            {
                stackParticipants.Children.Clear();
                stackParticipants.Children.Add(new Label
                {
                    Text = place.Invited[i].User.FirstName +
                            place.Invited[i].User.LastName + '(' +
                            place.Invited[i].EventRole.Title + ')',
                    Style = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Lbl"] as Style,
                    TextColor = Color.Blue,
                });
            }
            #endregion
            #region set people who want to take part in this place
            for (int i = 0; i < place.Wishers.Count; i++)
            {
                stackParticipants.Children.Clear();
                stackParticipants.Children.Add(new Label
                {
                    Text = place.Wishers[i].User.FirstName +
                            place.Wishers[i].User.LastName + '(' +
                            place.Wishers[i].EventRole.Title + ')',
                    Style = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Lbl"] as Style,
                    TextColor = Color.SandyBrown,
                });
            }
            #endregion
            #region set equipment on this place
            for (int i = 0; i < place.Equipment.Count; i++)
            {
                var response = await Services.HttpClientFactory.HttpClient.GetStringAsync($"user/{place.Equipment[i].OwnerId}");
                var owner = JsonConvert.DeserializeObject<OneObjectResponse<UserView>>(response);
                string ownerName = null;

                if (owner.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    ownerName = "Can't get owner";
                else if (owner.Data.Id == null)
                    ownerName = Resource.ADMIN_Laboratory;
                else
                    ownerName = owner.Data.LastName + ' ' + owner.Data.FirstName;

                stackEquipment.Children.Clear();
                stackEquipment.Children.Add(new Label
                {

                    Text = place.Equipment[i].EquipmentType.Title +
                           '(' + ownerName + ')',
                });
            }
            #endregion

            #region set 'TapGestureRecognizer' for Label which show titles of participants or equipment
            var tgr = new TapGestureRecognizer();
            tgr.Tapped += (s, e) =>
            {
                if (((Label)s).Text.Equals($"{Resource.Participants}:"))
                    stackParticipants.IsVisible = !stackParticipants.IsVisible;
                else if (((Label)s).Text.Equals($"{Resource.Equipment}:"))
                    stackEquipment.IsVisible = !stackEquipment.IsVisible;
            };
            lblParticipants.GestureRecognizers.Add(tgr);
            lblEquipment.GestureRecognizers.Add(tgr);
            #endregion
        }
    }
}