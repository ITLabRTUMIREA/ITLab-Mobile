using Http_Post.Classes;
using Models.PublicAPI.Requests.Account;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Http_Post
{
	public partial class Menu : MasterDetailPage
    {
        private Localization localization = new Localization();

        public Menu ()
		{
			InitializeComponent ();
        }

        public Menu(OneObjectResponse<LoginResponse> student)
        {
            InitializeComponent();

            label_name.Text = student.Data.FirstName;
            label_surname.Text = student.Data.LastName;

            //UpdateLanguage(); ------ Доделать
        }

        private async void LogOut_Clicked (object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync(true); // Go to MainPage --- ( Login )
        }

        private async void Settings_Clicked(object sender, EventArgs e)
        {
            // TODO: Titles

            Detail = new NavigationPage (new Settings()); // Load Settings Page
        }

        private void Lang_Clicked(object sender, EventArgs e)
        {
            AskForLanguage(localization);
        }

        private async void AskForLanguage(Localization loc)
        {
            try
            {
                string cancel = "Cancel";
                string result = await DisplayActionSheet("Choose language", cancel, String.Empty, loc.languages);

                if (result.Equals(cancel))
                    return;

                result = result.ToUpper();
                loc.ChangeCulture(result);

                UpdateLanguage();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private void UpdateLanguage()
        {
            // TODO: update all fields
            throw new NotImplementedException();
        }
    }
}