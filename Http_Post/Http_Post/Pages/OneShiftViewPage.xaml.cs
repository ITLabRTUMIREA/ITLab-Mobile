using Http_Post.Controls;
using Models.PublicAPI.Responses.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Http_Post.Pages
{
	public partial class OneShiftViewPage : ContentPage
	{
        ShiftView shift;
        List<PlaceView> places;

		public OneShiftViewPage (ShiftView tappedShift)
		{
			InitializeComponent ();
            UpdateLanguage();
            shift = tappedShift;
            places = shift.Places;
            SetProperties();
		}

        private void SetProperties()
        {
            Title = shift.BeginTime.ToString("dd MMMM, yyyy. HH: mm");
            lblDescription.Text = shift.Description;
            lblStart.Text = shift.BeginTime.ToString("dd MMMM, yyyy. HH:mm");
            lblEnd.Text = shift.EndTime.ToString("dd MMMM, yyyy. HH:mm");
            ///////////////////////////////////////
            var ps = new List<TextCell>();
            for (int i = 0; i < places.Count; i++)
            {
                var pview = new TextCell();
                pview.Text = $"Place №{(i + 1).ToString()}";
                pview.TextColor = lblDescription.TextColor;
                ps.Add(pview);
            }
            listView.ItemsSource = ps;
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // Detect which place user has tapped
            // Index of place required for Title in new page (better view for user)
            // Load new Page
            string tapped = (e.Item as TextCell).Text;
            int index = tapped[tapped.Length - 1] - 49; // I don't know why Convert.ToInt32 doesn't work
            var place = places[index];
            Navigation.PushAsync(new OnePlaceViewPage(place, index + 1));
        }

        private void UpdateLanguage()
        {
            lblPlaceHeader.Text = "Places:";
        }
    }
}