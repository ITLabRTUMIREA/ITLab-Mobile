using Http_Post.Res;
using Models.PublicAPI.Requests.Events.Event.Edit;
using Models.PublicAPI.Responses.Event;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Http_Post.Controls
{
    class StackShiftView
    {
        public StackLayout stackLayout;
        List<ShiftView> shifts = new List<ShiftView>();
        bool isCreating;
        
        public StackShiftView (List<ShiftView> views, bool isCreating = false)
        {
            shifts = views;
            this.isCreating = isCreating;

            GenerateStackForViewing();
        }

        void GenerateStackForViewing()
        {
            stackLayout = layout();

            int number = 1;
            foreach(var shift in shifts)
            {
                StackLayout oneShiftStack = OneShift(shift);
                oneShiftStack.IsVisible = false;

                StackLayout horizontal = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                };
                Image icon = Collapse;
                Label title = lab($"{Resource.Shifts} #{number}");
                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    oneShiftStack.IsVisible = !oneShiftStack.IsVisible; // change visibility
                    title.FontAttributes = oneShiftStack.IsVisible ? FontAttributes.Bold : FontAttributes.None;
                    icon.Source = oneShiftStack.IsVisible ? Expand.Source : Collapse.Source; // change icon
                };
                icon.GestureRecognizers.Add(tap);
                title.GestureRecognizers.Add(tap);
                // TODO: add delete option
                horizontal.Children.Add(title);
                horizontal.Children.Add(icon);

                stackLayout.Children.Add(horizontal);
                stackLayout.Children.Add(oneShiftStack);
                stackLayout.Children.Add(new BoxView
                {
                    HeightRequest = 1,
                    BackgroundColor = Color.Silver,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                });

                number++;
            }
        }

        StackLayout OneShift(ShiftView shift)
        {
            StackLayout oneShift = layout();
            
            if (isCreating)
            {
                DatePicker datePicker = new DatePicker
                {
                    Date = shift.BeginTime.Date
                };
                TimePicker timePicker = new TimePicker
                {
                    Time = shift.BeginTime.ToLocalTime().TimeOfDay
                };
                var hor = layout();
                hor.Orientation = StackOrientation.Horizontal;
                hor.Children.Add(lab("Begin:"));
                hor.Children.Add(datePicker);
                hor.Children.Add(timePicker);
                oneShift.Children.Add(hor);
            }
            else
                oneShift.Children.Add(lab(shift.BeginTime.ToLocalTime().ToString()));

            if (isCreating)
            {
                DatePicker datePicker = new DatePicker
                {
                    Date = shift.EndTime.Date
                };
                TimePicker timePicker = new TimePicker
                {
                    Time = shift.EndTime.ToLocalTime().TimeOfDay
                };
                var hor = layout();
                hor.Orientation = StackOrientation.Horizontal;
                hor.Children.Add(lab("End:"));
                hor.Children.Add(datePicker);
                hor.Children.Add(timePicker);
                oneShift.Children.Add(hor);
            }
            else
                oneShift.Children.Add(lab(shift.EndTime.ToLocalTime().ToString()));

            if(isCreating)
                oneShift.Children.Add(edit(string.IsNullOrEmpty(shift.Description) ? "" : shift.Description, Resource.Description));
            else
                oneShift.Children.Add(lab(string.IsNullOrEmpty(shift.Description) ? Resource.ErrorNoDescription : shift.Description));

            int number = 1;
            foreach(var place in shift.Places)
            {
                StackLayout onePlaceStack = OnePlace(place);
                onePlaceStack.IsVisible = false;

                StackLayout horizontal = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                };
                Image icon = Collapse;
                Label title = lab($"{Resource.Place} #{number}");
                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    onePlaceStack.IsVisible = !onePlaceStack.IsVisible; // change visibility
                    title.FontAttributes = onePlaceStack.IsVisible ? FontAttributes.Bold : FontAttributes.None;
                    icon.Source = onePlaceStack.IsVisible ? Expand.Source : Collapse.Source; // change icon
                };
                icon.GestureRecognizers.Add(tap);
                title.GestureRecognizers.Add(tap);
                // TODO: add delete option
                horizontal.Children.Add(title);
                horizontal.Children.Add(icon);

                oneShift.Children.Add(horizontal);
                oneShift.Children.Add(onePlaceStack);

                number++;
            }

            return oneShift;
        }

        StackLayout OnePlace(PlaceView place)
        {
            StackLayout onePlace = layout();

            onePlace.Children.Add(lab($"{Resource.Participants}: " + place.Participants.Count.ToString() + $" {Resource.Of} " + place.TargetParticipantsCount.ToString()));

            if(isCreating)
                onePlace.Children.Add(edit(string.IsNullOrEmpty(place.Description) ? "" : place.Description, Resource.Description));
            else
                onePlace.Children.Add(lab(string.IsNullOrEmpty(place.Description) ? Resource.ErrorNoDescription : place.Description));

            Label participantsTitle = lab(Resource.Participants + ":");
            participantsTitle.FontAttributes = FontAttributes.Bold;
            onePlace.Children.Add(participantsTitle);
            AddUsers(ref onePlace, place.Participants);
            AddUsers(ref onePlace, place.Invited);
            AddUsers(ref onePlace, place.Wishers);
            AddUsers(ref onePlace, place.Unknowns);

            Label equipmentTile = lab(Resource.TitleEquipment + ":");
            equipmentTile.FontAttributes = FontAttributes.Bold;
            onePlace.Children.Add(equipmentTile);
            foreach (var equipment in place.Equipment)
            {
                Label type = lab(equipment.EquipmentType.Title);
                type.Margin = new Thickness(10,0,0,0);
                Label serial = lab(equipment.SerialNumber);
                serial.Margin = new Thickness(10, 0, 0, 0);
                serial.FontSize = type.FontSize - 4;
                serial.FontAttributes = FontAttributes.Italic;

                onePlace.Children.Add(type);
                onePlace.Children.Add(serial);
            }

            return onePlace;
        }

        void AddUsers(ref StackLayout stack, List<UserAndEventRole> users)
        {
            foreach (var user in users)
            {
                Label name = lab(user.User.FirstName + " " + user.User.LastName);
                name.Margin = new Thickness(10, 0, 0, 0);
                Label role = lab(user.EventRole.Title);
                role.Margin = new Thickness(10, 0, 0, 0);
                role.FontSize = name.FontSize - 4;
                role.FontAttributes = FontAttributes.Italic;

                stack.Children.Add(name);
                stack.Children.Add(role);
            }
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

        StackLayout layout()
        {
            return new StackLayout();
        }

        Label lab(string txt)
        {
            return new Label
            {
                Text = txt
            };
        }

        Editor edit(string txt = "", string holder = "")
        {
            return new Editor
            {
                Text = txt,
                Placeholder = holder,
            };
        }
    }
}
