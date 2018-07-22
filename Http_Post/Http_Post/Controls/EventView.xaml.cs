using System;
using Xamarin.Forms;

namespace Http_Post.Controls
{
	public partial class EventView : ViewCell
	{
        public EventView ()
        {
            InitializeComponent();

            //SetLayout();
        }

        private void SetLayout()
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
               // Phone.IsVisible = true;
                //Non_Phone.IsVisible = false;
            }
            else
            {
                //Phone.IsVisible = false;
                //Non_Phone.IsVisible = true;
            }
        }
    }
}