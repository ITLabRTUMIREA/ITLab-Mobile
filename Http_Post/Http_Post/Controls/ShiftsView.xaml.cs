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
	public partial class ShiftsView : ContentView
	{
        bool isEdit = false;
        ShiftView shift;
        bool Delete = false;
		public ShiftsView (ShiftView shift, int ShiftNumber, bool isEdit = false)
		{
			InitializeComponent ();
            this.shift = shift;
            this.isEdit = isEdit;
            shiftTitle.Text = Resource.Shifts + $" #{ShiftNumber}:";
            Language();

            int PlaceNumber = 1;
            foreach(var place in shift.Places)
            {
                placesStack.Children.Add(new PlacesView(PlaceNumber, place, isEdit));
                PlaceNumber++;
            }
        }

        void Language()
        {
            beginTitle.Text = isEdit ? "Begin:" : shift.BeginTime.ToLocalTime().ToString();
            endTitle.Text = isEdit ? "End:" : shift.EndTime.ToLocalTime().ToString();
            lblDescription.Text = string.IsNullOrEmpty(shift.Description) ? Resource.ErrorNoDescription : shift.Description;

            if(isEdit)
            {
                lblDescription.IsVisible = false;
                editDescription.IsVisible = true;
                editDescription.Text = shift.Description;
                editDescription.Placeholder = Resource.Description;
                //
                ImageDelete.IsVisible = true;
                ImageEdit.IsVisible = true;
                //
                dateBegin.IsVisible = true;
                dateBegin.Date = shift.BeginTime.Date;
                timeBegin.IsVisible = true;
                timeBegin.Time = shift.BeginTime.ToLocalTime().TimeOfDay;
                //
                dateEnd.IsVisible = true;
                dateEnd.Date = shift.EndTime.Date;
                timeEnd.IsVisible = true;
                timeEnd.Time = shift.EndTime.ToLocalTime().TimeOfDay;
            }
        }

        void Tap_ShowHide(object sender, EventArgs e)
        {
            placesStack.IsVisible = !placesStack.IsVisible; // change visibility
            beginStack.IsVisible = !beginStack.IsVisible;
            endStack.IsVisible = !endStack.IsVisible;
            stackDescription.IsVisible = !stackDescription.IsVisible;
            shiftTitle.FontAttributes = placesStack.IsVisible ? FontAttributes.Bold : FontAttributes.None;
            ImageShowHide.Source = placesStack.IsVisible ? Expand.Source : Collapse.Source; // change icon
        }

        void Tap_Delete(object sender, EventArgs e)
        {
            shiftTitle.BackgroundColor = Color.FromHex("#ff8080");
            Delete = true;
        }

        void Tap_Edit(object sender, EventArgs e)
        {

        }

        public Models.PublicAPI.Requests.Events.Event.Edit.ShiftEditRequest shiftEdit { get
            {
                var places = new List<Models.PublicAPI.Requests.Events.Event.Edit.PlaceEditRequest>();
                foreach (PlacesView place in placesStack.Children)
                {
                    places.Add(place.placeEdit);
                }
                if (Delete)
                    return new Models.PublicAPI.Requests.Events.Event.Edit.ShiftEditRequest {
                        Id = shift.Id,
                        Delete = this.Delete
                    };
                else
                    return new Models.PublicAPI.Requests.Events.Event.Edit.ShiftEditRequest
                    {
                        Id = shift.Id,
                        Description = editDescription.Text,
                        BeginTime = dateBegin.Date + timeBegin.Time,
                        EndTime = dateEnd.Date + timeEnd.Time,
                        Places = places
                    };
            }
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