using Models.PublicAPI.Responses.Event;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShiftContentView : ContentView
	{
        bool isDown;
        ShiftView shift;

		public ShiftContentView (ShiftView shiftView, int numberOfShift)
		{
            shift = shiftView;
			InitializeComponent ();

            lblShiftNumber.Text = $"#{numberOfShift} | {Res.Resource.Places}: {shiftView.Places.Count}";
            image.Source = Images.ImageManager.GetSourceImage("ArrowDown");
            isDown = true;

            lblBeginTitle.Text = $"{Res.Resource.Begining}:";
            lblBeginTime.Text = shiftView.BeginTime.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
            lblEndTitle.Text = $"{Res.Resource.Ending}:";

            lblEndTime.Text = shiftView.EndTime.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
            lblDescription.Text = string.IsNullOrEmpty(shiftView.Description) ? Res.Resource.ErrorNoDescription : shiftView.Description;

            AddPlaces();
		}

        void ShiftNumber_Tapped(object sender, EventArgs e)
        {
            isDown = !isDown;
            StackPlaces.IsVisible = !StackPlaces.IsVisible;
            image.Source = isDown ? Images.ImageManager.GetSourceImage("ArrowDown")
                : Images.ImageManager.GetSourceImage("ArrowUp");
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