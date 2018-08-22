using Http_Post.Classes;
using Http_Post.Pages;
using Http_Post.Res;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.Login;
using System;
using Xamarin.Forms;

namespace Http_Post
{
	public partial class Menu : MasterDetailPage
    {
        private Localization localization = new Localization();

        private OneObjectResponse<LoginResponse> student;

        private string Name;
        private string LastName;

        public Menu(OneObjectResponse<LoginResponse> student)
        {
            InitializeComponent();

            this.student = student;
            InitComponents();

            UpdateLanguage();
            UpdateTheme();

            BtnEvents_Clicked(Btn_Events, EventArgs.Empty);
        }

        private void InitComponents()
        {
            Name = student.Data.FirstName;
            LastName = student.Data.LastName;

            label_name.Text = Name;
            label_surname.Text = LastName;
        }

        private void BtnEvents_Clicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new EventPage(student)) {
                Style = Application.Current.Resources[new ThemeChanger().Theme +"_Bar"] as Style
            }; // Load Events page
            Close();
        }

        private void BtnDateSearch_Clicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new DateSearchPage(student)) {
                Style = Application.Current.Resources[new ThemeChanger().Theme + "_Bar"] as Style
            }; // Load Date searching page
            Close();
        }

        private void BtnCreate_Clicked (object sender, EventArgs e)
        {
            Detail = new NavigationPage(new CreateEventPage()) {
                Style = Application.Current.Resources[new ThemeChanger().Theme + "_Bar"] as Style
            }; // Load Creation page
            Close();
        }

        private void Settings_Clicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new Settings(this)) {
                Style = Application.Current.Resources[new ThemeChanger().Theme + "_Bar"] as Style
            }; // Load Settings page

            Close();
        }

        private async void AboutUs(object sender, EventArgs e)
        {
            await DisplayAlert("About", "Change with Resources!\nHelp us not to die\nMy name is Your Name\nBye", "Ok");
            Close();
        }

        public void UpdateLanguage()
        {
            Btn_Events.Btn_Text = Resource.Btn_Events;
            Btn_Settings.Btn_Text = Resource.Btn_Settings;
            Btn_About.Btn_Text = Resource.Btn_About;
            Btn_Create.Btn_Text = Resource.Btn_Create;
            Btn_DateSearch.Btn_Text = Resource.Btn_DateSearch;
        }

        public void UpdateTheme()
        {
            var col = Application.Current.Resources;
            var th = new ThemeChanger().Theme;
            col["themeStack"] = col[th + "_Stack"];
            col["themeLabel"] = col[th + "_Lbl"];

            Btn_Events.UpdateTheme();
            Btn_Settings.UpdateTheme();
            Btn_About.UpdateTheme();
            Btn_Create.UpdateTheme();
            Btn_DateSearch.UpdateTheme();
        }

        public async void Logout()
            => await Navigation.PopToRootAsync(true);

        // Close menu
        private void Close()
        {
            IsPresented = false;
        }
    }
}