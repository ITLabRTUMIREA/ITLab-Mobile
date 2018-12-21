using Models.PublicAPI.Requests.Events.Event.Edit;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShiftEditContentView : ContentView
	{
        Image pathCollapse = new Image
        {
            Source = "ArrowRight.png"
        };
        Image pathExpand = new Image
        {
            Source = "ArrowDown.png"
        };
        ShiftEditRequest shiftEdit;
        int numberOfShift;

        public ShiftEditContentView (ShiftEditRequest shiftEditRequest, int numberOfShift)
		{
            this.numberOfShift = numberOfShift;
            shiftEdit = shiftEditRequest;
			InitializeComponent ();

            image.Source = pathCollapse.Source;

            lblShiftNumber.Text = $"#{numberOfShift} | {Res.Resource.Places}: {shiftEdit.Places.Count}";
            lblBeginTitle.Text = "Begining time:";
            DateTime date = shiftEdit.BeginTime.Value;
            DateTime date1 = shiftEditRequest.BeginTime.Value;
            DatePickerBegin.Date = shiftEdit.BeginTime.Value.ToLocalTime().Date;
            TimePickerBegin.Time = shiftEdit.BeginTime.Value.ToLocalTime().TimeOfDay;

            lblEndTitle.Text = "Ending time:";
            DatePickerEnd.Date = shiftEdit.EndTime.Value.ToLocalTime().Date;
            TimePickerEnd.Time = shiftEdit.EndTime.Value.ToLocalTime().TimeOfDay;

            editDescription.Placeholder = Res.Resource.Description;
            editDescription.Text = shiftEdit.Description;

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
            for (int i = 6; i < StackPlaces.Children.Count; i++)
                StackPlaces.Children.RemoveAt(i);
            int num = 1;
            foreach(var place in shiftEdit.Places)
            {
                if (place.Delete)
                    continue;
                StackPlaces.Children.Add(new PlaceEditContentView(place, numberOfShift, num));
                num++;
            }
            lblShiftNumber.Text = $"#{numberOfShift} | {Res.Resource.Places}: {num - 1}";
        }

        void DeleteShift_Tapped(object sender, EventArgs e)
        {
            shiftEdit.Delete = true;
            this.IsVisible = false;
        }

        async void CreatePlace_Tapped(object sender, EventArgs e)
        {
            PlaceEditRequest placeEditRequest = await new Popup.Event.CreatePlace().AddPlaceEditRequest(Navigation);
            if (placeEditRequest == null)
                return;

            shiftEdit.Places.Add(placeEditRequest);
            AddPlaces();
        }

        void editDescription_TextChanged(object sender, TextChangedEventArgs e)
            => shiftEdit.Description = editDescription.Text;

        void DatePickerBegin_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            => shiftEdit.BeginTime = DatePickerBegin.Date + TimePickerBegin.Time;

        private void TimePickerBegin_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            => shiftEdit.BeginTime = DatePickerBegin.Date + TimePickerBegin.Time;

        void DatePickerEnd_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            => shiftEdit.EndTime = DatePickerEnd.Date + TimePickerEnd.Time;

        void TimePickerEnd_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            => shiftEdit.EndTime = DatePickerEnd.Date + TimePickerEnd.Time;
    }
}