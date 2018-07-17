using Models.PublicAPI.Responses.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Http_Post.Controls
{
	public partial class EventView : ViewCell
	{
        public EventView ()
        {
            InitializeComponent();
            if (Device.Idiom == TargetIdiom.Phone)
            {
                Phone.IsVisible = true;
                Non_Phone.IsVisible = false;
            }
            else
            {
                Phone.IsVisible = false;
                Non_Phone.IsVisible = true;
            }
        }

        private async void BtnGo_Clicked (object sender, EventArgs e)
        {

        }

        private async void BtnWontGo_Clicked (object sender, EventArgs e)
        {

        }
    }
}