using Http_Post.Classes;
using System;
using Xamarin.Forms;

namespace Http_Post
{
	public partial class Settings : ContentPage
	{
        Localization localization = new Localization();

		public Settings ()
		{
			InitializeComponent ();
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
            DisplayAlert("Default", "Lang update", "Default", "Default");
            //throw new NotImplementedException();
        }

        private void Theme_Change (object sender, EventArgs e)
        {
            DisplayAlert("Default", "Theme update", "Default", "Default");
        }
    }
}