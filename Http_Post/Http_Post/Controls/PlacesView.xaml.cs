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
	public partial class PlacesView : ContentView
	{
        bool isEdit = false;
        PlaceView place;

        Style styleStack = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Stack"] as Style;
        Style styleLbl= Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Lbl"] as Style;

        public PlacesView (int placeNumber, PlaceView place, bool isEdit = false)
		{
			InitializeComponent ();
            this.isEdit = isEdit;
            this.place = place;
            placeTitle.Text = Resource.Place + $" #{placeNumber}:";
            Language();
            SetEquipmentAndUsers();
		}

        void Language()
        {
            target.Text = $"{Resource.Participants}: " + place.Participants.Count.ToString() + $" {Resource.Of} " + place.TargetParticipantsCount.ToString();
            LblDescription.Text = string.IsNullOrEmpty(place.Description) ? Resource.ErrorNoDescription : place.Description;
            participants.Text = Resource.Participants + ":";
            equipment.Text = Resource.TitleEquipment + ":";
            if(isEdit)
            {
                EditDescription.IsVisible = true;
                EditDescription.Placeholder = Resource.Description;
                EditDescription.Text = place.Description;
                // images
                ImageDelete.IsVisible = true;
                ImageEdit.IsVisible = true;
            }
        }

        void SetEquipmentAndUsers()
        {
            // equipments
            var EquipmentCells = new List<ViewCell>();
            foreach (var equipment in place.Equipment)
            {
                Label type = lab(equipment.EquipmentType.Title);
                type.Margin = new Thickness(10, 0, 0, 0);
                Label serial = lab(equipment.SerialNumber);
                serial.Margin = new Thickness(10, 0, 0, 0);
                serial.FontSize = type.FontSize - 4;
                serial.FontAttributes = FontAttributes.Italic;

                var layout = new StackLayout
                {
                    Style = styleStack,
                    Children = { type, serial },
                };
                EquipmentCells.Add(new ViewCell
                {
                    View = layout
                });
            }
            Equipment.ItemsSource = EquipmentCells;

            // participants
            var ParticipantsCells = new List<ViewCell>();
            ParticipantsCells.Add(new ViewCell
            {
                View = AddUsers(place.Participants)
            });
            ParticipantsCells.Add(new ViewCell
            {
                View = AddUsers(place.Invited)
            });
            ParticipantsCells.Add(new ViewCell
            {
                View = AddUsers(place.Wishers)
            });
            ParticipantsCells.Add(new ViewCell
            {
                View = AddUsers(place.Unknowns)
            });
            Participants.ItemsSource = ParticipantsCells;
        }

        StackLayout AddUsers(List<UserAndEventRole> users)
        {
            var layout = new StackLayout
            {
                Style = styleStack
            };

            foreach (var user in users)
            {
                Label name = lab(user.User.FirstName + " " + user.User.LastName);
                name.Margin = new Thickness(10, 0, 0, 0);
                Label role = lab(user.EventRole.Title);
                role.Margin = new Thickness(10, 0, 0, 0);
                role.FontSize = name.FontSize - 4;
                role.FontAttributes = FontAttributes.Italic;

                layout.Children.Add(name);
                layout.Children.Add(role);
            }
            return layout;
        }

        void Tap_ShowHide(object sender, EventArgs e)
        {
            stackShowHide.IsVisible = !stackShowHide.IsVisible; // change visibility
            placeTitle.FontAttributes = stackShowHide.IsVisible ? FontAttributes.Bold : FontAttributes.None;
            ImageShowHide.Source = stackShowHide.IsVisible ? Expand.Source : Collapse.Source; // change icon
        }

        void Tap_Delete(object sender, EventArgs e)
        {

        }

        void Tap_Edit(object sender, EventArgs e)
        {

        }

        Label lab(string txt)
        {
            return new Label
            {
                Style = styleLbl,
                Text = txt
            };
        }

        Image Collapse
        {
            get
            {
                return new Image
                {
                    Source = "Keyboard_arrow_right_black.png",
                    WidthRequest = 24,
                    HeightRequest = 24,
                    Aspect = Aspect.AspectFit
                };
            }
        }

        Image Expand
        {
            get
            {
                return new Image
                {
                    Source = "Keyboard_arrow_down_black.png",
                    WidthRequest = 24,
                    HeightRequest = 24,
                    Aspect = Aspect.AspectFit
                };
            }
        }
    }
}