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

        string cancel = "Cancel";

        public Settings()
        {
            InitializeComponent();
        }

        public Settings(Menu menu)
        {
            InitializeComponent();
            menuSet = menu;
            UpdateLanguage();
            UpdateTheme();
        }

        private void Lang_Clicked(object sender, EventArgs e)
            => AskForLanguage(localization);

        private async void AskForLanguage(Localization loc)
        {
            try
            {
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
            Title = Resource.Title_Settings;
            Btn_Theme.Btn_Text = Resource.Btn_Theme;
            Btn_LogOut.Btn_Text = Resource.Btn_LogOut;
        }

        private async void Theme_Change (object sender, EventArgs e)
        {
            try
            {
                var themeChanger = new ThemeChanger();

                string result = await DisplayActionSheet(Resource.ThemeChoose, cancel, String.Empty, themeChanger.ThemesForUser.ToArray());
                if (result.Equals(cancel))
                    return;

                themeChanger.ChangeTheme(result);
                UpdateTheme();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private void UpdateTheme()
        {
            Application.Current.Resources["themeStack"] = Application.Current.Resources
                [new ThemeChanger().Theme + "_Stack"];

            Btn_Lang.UpdateTheme();
            Btn_Theme.UpdateTheme();
            Btn_LogOut.UpdateTheme();

            menuSet.UpdateTheme();
        }

        private void LogOut_Clicked(object sender, EventArgs e)
            => menuSet.Logout();
    }
}