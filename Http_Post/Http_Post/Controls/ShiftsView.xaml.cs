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
                PlaceNumber++;
                placesStack.Children.Add(new PlacesView(PlaceNumber, place, isEdit));
            }
        }

        void Language()
        {
            beginTitle.Text = isEdit ? "Begin:" : shift.BeginTime.ToLocalTime().ToString();
            endTitle.Text = isEdit ? "End:" : shift.EndTime.ToLocalTime().ToString();

            if(isEdit)
            {
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
            shiftTitle.FontAttributes = placesStack.IsVisible ? FontAttributes.Bold : FontAttributes.None;
            ImageShowHide.Source = placesStack.IsVisible ? Expand.Source : Collapse.Source; // change icon
        }

        void Tap_Delete(object sender, EventArgs e)
        {

        }

        void Tap_Edit(object sender, EventArgs e)
        {

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