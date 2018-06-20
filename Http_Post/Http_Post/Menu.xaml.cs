using Models.PublicAPI.Requests.Account;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post
{
	public partial class Menu : ContentPage
	{
		public Menu ()
		{
			InitializeComponent ();
		}

        public Menu(OneObjectResponse<LoginResponse> student)
        {
            InitializeComponent();

            label_name.Text = student.Data.FirstName;
            label_surname.Text = student.Data.LastName;
        }
	}
}