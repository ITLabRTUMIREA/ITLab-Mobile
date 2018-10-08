using Http_Post.Res;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Http_Post.Pages
{
	public partial class OnePlaceViewPage : ContentPage
	{
        PlaceView place;
		public OnePlaceViewPage (PlaceView place, int index)
		{
			InitializeComponent ();
            Title = "Place №" + index;
            this.place = place;

            UpdateLanguage();
            SetProperties();
		}

        void UpdateLanguage()
        {
            lblParticipants.Text = "Participants" + ':';
            lblEquipment.Text = "Equipment" + ':';
            lblDescription.Text = place.Description;
            lblTarget.Text = place.Participants.Count.ToString() + " of " + place.TargetParticipantsCount.ToString();
        }

        async void SetProperties()
        {
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

            var tgr = new TapGestureRecognizer();
            tgr.Tapped += (s, e) =>
            {
                if (((Label)s).Text.Equals("Participants" + ':'))
                    stackParticipants.IsVisible = !stackParticipants.IsVisible;
                else if (((Label)s).Text.Equals("Equipment" + ':'))
                    stackEquipment.IsVisible = !stackEquipment.IsVisible;
            };
            lblParticipants.GestureRecognizers.Add(tgr);
            lblEquipment.GestureRecognizers.Add(tgr);
        }
	}
}