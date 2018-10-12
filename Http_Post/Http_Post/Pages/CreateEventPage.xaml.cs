using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Http_Post.Pages
{
	public partial class CreateEventPage : ContentPage
	{
		public CreateEventPage ()
		{
			InitializeComponent ();
            UpdateLanguage();
		}

        private void UpdateLanguage()
        {
            Title = Res.Resource.Title_Create;
            lblEventType.Text = "Event Type";
            lblName.Text = "Name";
            lblDescription.Text = "Description";
            lblAddress.Text = "Address";
            btnAddShift.Text = "Shifts";
            ///////////////////////////////////////
            editEventType.Placeholder = lblEventType.Text;
            editName.Placeholder = lblName.Text;
            editDescription.Placeholder = lblDescription.Text;
            editAddress.Placeholder = lblAddress.Text;
        }
	}
}