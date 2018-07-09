using Http_Post.Classes;
using Http_Post.Res;
using System;
using Xamarin.Forms;

namespace Http_Post
{
	public partial class Settings : ContentPage
	{
        Localization localization = new Localization();
        Menu menuSet;

		public Settings ()
		{
			InitializeComponent ();
		}

        public Settings (Menu menu)
        {
            InitializeComponent();
            menuSet = menu;
            UpdateLanguage();
        }

        private void Lang_Clicked(object sender, EventArgs e) 
            => AskForLanguage(localization);

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
                menuSet.UpdateLanguage();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private void UpdateLanguage()
        {
            //DisplayAlert("Default", "Lang update", "Default", "Default");
            Btn_Theme.Btn_Text = Resource.Btn_Theme;
            Btn_LogOut.Btn_Text = Resource.Btn_LogOut;

            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! //
            Btn_LogOut.Back = "ff8080"; // Red Color for 'Log out' Button
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! //
        }

        private void Theme_Change (object sender, EventArgs e)
        {
            DisplayAlert("Default", "Theme update", "Default", "Default");
        }
        
        private void LogOut_Clicked(object sender, EventArgs e)
            => menuSet.Logout();
            //=> await Navigation.PopToRootAsync(true); // Go to MainPage --- ( Login )
    }
}