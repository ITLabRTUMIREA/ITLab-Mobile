using Models.PublicAPI.Requests.Events.Event.Edit;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShiftEditContentView : ContentView
	{
        bool isDown;
        ShiftEditRequest shiftEdit = new ShiftEditRequest();
        int numberOfShift;

        public ShiftEditContentView (ShiftEditRequest shiftEditRequest, int numberOfShift)
		{
            this.numberOfShift = numberOfShift;
			InitializeComponent ();

            image.Source = Images.ImageManager.GetSourceImage("ArrowDown");
            isDown = true;

            lblShiftNumber.Text = $"#{numberOfShift} | {Res.Resource.Places}: {shiftEdit.Places.Count}";
            lblBeginTitle.Text = $"{Res.Resource.Begining}:";
            DatePickerBegin.Date = shiftEditRequest.BeginTime.Value.ToLocalTime().Date;
            TimePickerBegin.Time = shiftEditRequest.BeginTime.Value.ToLocalTime().TimeOfDay;

            lblEndTitle.Text = $"{Res.Resource.Ending}:";
            DatePickerEnd.Date = shiftEditRequest.EndTime.Value.ToLocalTime().Date;
            TimePickerEnd.Time = shiftEditRequest.EndTime.Value.ToLocalTime().TimeOfDay;

            shiftEdit = shiftEditRequest;
            editDescription.Placeholder = Res.Resource.Description;
            editDescription.Text = shiftEdit.Description;

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