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
        bool Delete = false;

        Style styleStack = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Stack"] as Style;
        Style styleLbl = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Lbl"] as Style;

        public PlacesView(int placeNumber, PlaceView place, bool isEdit = false)
        {
            InitializeComponent();
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
            if (isEdit)
            {
                LblDescription.IsVisible = false;
                EditDescription.IsVisible = true;
                EditDescription.Placeholder = Resource.Description;
                EditDescription.Text = place.Description;
                // images
                ImageDelete.IsVisible = true;
                ImageEdit.IsVisible = true;
                // target
                editTarget.Text = place.TargetParticipantsCount.ToString();
                editTarget.IsVisible = true;
            }
        }

        void SetEquipmentAndUsers()
        {
            foreach (var equipment in place.Equipment)
            {
                Label type = lab(equipment.EquipmentType.Title);
                type.Margin = new Thickness(10, 0, 0, 0);
                Label serial = lab(equipment.SerialNumber);
                serial.Margin = new Thickness(10, 0, 0, 0);
                serial.FontSize = type.FontSize - 4;
                serial.FontAttributes = FontAttributes.Italic;

                Equipment.Children.Add(type);
                Equipment.Children.Add(serial);
            }
            AddUsers(place.Participants);
            AddUsers(place.Invited);
            AddUsers(place.Wishers);
            AddUsers(place.Unknowns);
        }

        void AddUsers(List<UserAndEventRole> users)
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

                Participants.Children.Add(name);
                Participants.Children.Add(role);

            }
        }

        void Tap_ShowHide(object sender, EventArgs e)
        {
            stackShowHide.IsVisible = !stackShowHide.IsVisible; // change visibility
            placeTitle.FontAttributes = stackShowHide.IsVisible ? FontAttributes.Bold : FontAttributes.None;
            ImageShowHide.Source = stackShowHide.IsVisible ? Expand.Source : Collapse.Source; // change icon
        }

        void Tap_Delete(object sender, EventArgs e)
        {
            placeTitle.BackgroundColor = Color.FromHex("#ff8080");
            Delete = true;
        }

        void Tap_Edit(object sender, EventArgs e)
        {

        }

        void editTarget_TextChanged(object sender, TextChangedEventArgs e)
        {
            target.Text = $"{Resource.Participants}: " + place.Participants.Count.ToString() + $" {Resource.Of} " + Convert.ToInt32(editTarget.Text);
        }

        public Models.PublicAPI.Requests.Events.Event.Edit.PlaceEditRequest placeEdit
        {
            get
            {
                if (Delete)
                    return new Models.PublicAPI.Requests.Events.Event.Edit.PlaceEditRequest
                    {
                        Id = place.Id,
                        Delete = this.Delete
                    };
                else
                    return new Models.PublicAPI.Requests.Events.Event.Edit.PlaceEditRequest
                    {
                        Id = place.Id,
                        Description = EditDescription.Text,
                        TargetParticipantsCount = Convert.ToInt32(editTarget.Text),
                        // TODO: make it done -- equipment, participants
                    };
            }
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
                    Source = "ArrowRight.png",
                    WidthRequest = 18,
                    HeightRequest = 18,
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
                    Source = "ArrowDown.png",
                    WidthRequest = 18,
                    HeightRequest = 18,
                    Aspect = Aspect.AspectFit
                };
            }
        }
    }
}