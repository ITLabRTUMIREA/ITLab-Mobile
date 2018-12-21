using Models.PublicAPI.Responses.Event;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShiftContentView : ContentView
	{
        Image pathCollapse = new Image
        {
            Source = "ArrowRight.png"
        };
        Image pathExpand = new Image
        {
            Source = "ArrowDown.png"
        };
        ShiftView shift;

		public ShiftContentView (ShiftView shiftView, int numberOfShift)
		{
            shift = shiftView;
			InitializeComponent ();

            lblShiftNumber.Text = $"#{numberOfShift} | {Res.Resource.Places}: {shiftView.Places.Count}";
            image.Source = pathCollapse.Source;

            lblBeginTitle.Text = $"{Res.Resource.Begining}:";
            lblBeginTime.Text = shiftView.BeginTime.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
            lblEndTitle.Text = $"{Res.Resource.Ending}:";

            lblEndTime.Text = shiftView.EndTime.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
            lblDescription.Text = string.IsNullOrEmpty(shiftView.Description) ? Res.Resource.ErrorNoDescription : shiftView.Description;

            AddPlaces();
		}

        void ShiftNumber_Tapped(object sender, EventArgs e)
        {
            if (image.Source.Equals(pathCollapse.Source))
                image.Source = pathExpand.Source;
            else
                image.Source = pathCollapse.Source;

            StackPlaces.IsVisible = !StackPlaces.IsVisible;
        }

        void AddPlaces()
        {
            int num = 1;
            foreach (var place in shift.Places)
            {
                StackPlaces.Children.Add(new PlaceContentView(place, num));
                num++;
            }
        }
    }
}