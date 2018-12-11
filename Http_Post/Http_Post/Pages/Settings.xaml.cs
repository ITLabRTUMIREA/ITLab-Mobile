using Http_Post.Classes;
using Http_Post.Pages;
using Http_Post.Res;
using Http_Post.Styles;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
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
        }

        private void Lang_Clicked(object sender, EventArgs e)
            => AskForLanguage(localization);

        private async void AskForLanguage(Localization loc)
        {
            try
            {
                string result = await DisplayActionSheet("Choose language", cancel, null, loc.languages);

                if (string.IsNullOrEmpty(result) || result.Equals(cancel))
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
                //Application.Current.Resources.ChangeTheme();
                Application.Current.Resources.MergedDictionaries.Remove(ThemeManager.GetCurrentTheme());
                ThemeManager.ChangeTheme();
                Application.Current.Resources.MergedDictionaries.Add(ThemeManager.GetCurrentTheme());
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

        private void LogOut_Clicked(object sender, EventArgs e)
            => menuSet.Logout();

        private async void BtnProfile_Clicked(object sender, EventArgs e)
            => await Navigation.PushAsync(new ProfilePage());
    }
}