using Http_Post.Res;
using Models.PublicAPI.Responses.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Http_Post.Controls
{
	public partial class Place : ContentView
	{
		public Place (PlaceView place, int numberOfPlace)
		{
			InitializeComponent ();

            title.Text = $"{Resource.Place}# {numberOfPlace}";
            target.Text = $"{Resource.Participants}: " + place.Participants.Count.ToString() + $" {Resource.Of} " + place.TargetParticipantsCount.ToString(); // set how many participants now in this place
            description.Text = string.IsNullOrEmpty(place.Description) ? Resource.ErrorNoDescription : place.Description; // set description

            // participants
            Participants.Text = Resource.Participants;

            // equipment
            Equipment.Text = Resource.TitleEquipment;
        }

        public StackLayout stackLayout
        {
            get
            {
                return stack;
            }
        }
	}
}