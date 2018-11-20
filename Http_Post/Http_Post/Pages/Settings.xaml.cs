using Http_Post.Classes;
using Http_Post.Pages;
using Http_Post.Res;
using System;
using Xamarin.Forms;

namespace Http_Post
{
    public partial class Settings : ContentPage
    {
        Localization localization = new Localization();
        Menu menuSet;

        string cancel = Resource.ADMIN_Cancel;

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
                string result = await DisplayActionSheet("Choose language", cancel, null, loc.languages);

                if (result.Equals(cancel) || string.IsNullOrEmpty(result))
                    return;

                result = result.ToUpper();
                loc.ChangeCulture(result);

                UpdateLanguage();
                menuSet.UpdatePages();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private async void Theme_Change (object sender, EventArgs e)
        {
            try
            {
                var themeChanger = new ThemeChanger();

                string result = await DisplayActionSheet(Resource.ChangeTheme, cancel, null, themeChanger.ThemesForUser.ToArray());
                if (string.IsNullOrEmpty(result) || result.Equals(cancel))
                    return;

                themeChanger.ChangeTheme(result);
                UpdateTheme();
                var col = Application.Current.Resources;
                var th = themeChanger.Theme;
                col["themeStack"] = col[th + "_Stack"];
                col["themeLabel"] = col[th + "_Lbl"];
                col["themeButton"] = col[th + "_Btn"];
                col["themeBar"] = col[th + "_Bar"];
                col["themeTab"] = col[th + "_Tab"];
                //menuSet.UpdateTheme();
                //menuSet.UpdatePages();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private void UpdateLanguage()
        {
            Title = Device.RuntimePlatform == Device.UWP ? Resource.TitleSettings : "";
            Btn_Theme.Btn_Text = Resource.ChangeTheme;
            Btn_LogOut.Btn_Text = Resource.LogOut;
            Btn_Profile.Btn_Text = Resource.TitleProfile;
        }

        private void UpdateTheme()
        {
            Btn_Lang.UpdateTheme();
            Btn_Theme.UpdateTheme();
            Btn_LogOut.UpdateTheme();
        }

        private void LogOut_Clicked(object sender, EventArgs e)
            => menuSet.Logout();

        private async void BtnProfile_Clicked(object sender, EventArgs e)
            => await Navigation.PushAsync(new ProfilePage());
    }
}