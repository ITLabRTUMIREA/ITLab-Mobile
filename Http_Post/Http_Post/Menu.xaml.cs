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
	public partial class Menu : MasterDetailPage
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

        private async void LogOut_Clicked (object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync(true); // Go to MainPage --- ( Login )
        }

        private async void Settings_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Settings()); // Load Settings Page
        }

        private async void Lang_Clicked(object sender, EventArgs e)
        {
            // TODO: change language

            string[] languages = { "English", "Russian" };
            string result = await DisplayActionSheet("Choose language", "Cancel", null, languages);
        }
    }
}