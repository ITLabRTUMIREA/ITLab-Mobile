using Http_Post.Classes;
using Http_Post.Pages;
using Http_Post.Res;
using Http_Post.Services;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.Login;
using Models.PublicAPI.Responses.People;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using Xamarin.Forms;

namespace Http_Post
{
	public partial class Menu : MasterDetailPage
    {
        private Localization localization = new Localization();

        private HttpClient client = HttpClientFactory.HttpClient;
        private OneObjectResponse<LoginResponse> student;

        public Menu(OneObjectResponse<LoginResponse> student)
        {
            InitializeComponent();

            this.student = student;
            InitComponents();

            UpdateLanguage();
            UpdateTheme();

            BtnEvents_Clicked(Btn_Events, EventArgs.Empty);
        }

        public async void InitComponents()
        {
            try
            {
                var response = await client.GetStringAsync($"user/{student.Data.User.Id}");
                var currentUserObject = JsonConvert.DeserializeObject<OneObjectResponse<UserView>>(response);

                if (currentUserObject.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {currentUserObject.StatusCode}");

                label_name.Text = currentUserObject.Data.FirstName;
                label_surname.Text = currentUserObject.Data.LastName;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private void BtnEvents_Clicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new EventPage()) {
                Style = Application.Current.Resources[new ThemeChanger().Theme + "_Bar"] as Style,
            }; // Load Events page
            Close();
        }

        private void BtnDateSearch_Clicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new DateSearchPage()) {
                Style = Application.Current.Resources[new ThemeChanger().Theme + "_Bar"] as Style
            }; // Load Date searching page
            Close();
        }

        private void BtnCreateEvent_Clicked (object sender, EventArgs e)
        {
            Detail = new NavigationPage(new CreateEventPage()) {
                Style = Application.Current.Resources[new ThemeChanger().Theme + "_Bar"] as Style
            }; // Load Creation page
            Close();
        }

        private void BtnEquip_Clicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new EquipmentPage()) {
                Style = Application.Current.Resources[new ThemeChanger().Theme + "_Bar"] as Style
            }; // Load Equipment page
            Close();
        }

        private void BtnCreateEquip_Clicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new CreateEquipment())
            {
                Style = Application.Current.Resources[new ThemeChanger().Theme + "_Bar"] as Style,
            }; // Load Events page
            Close();
        }

        private void BtnTypes_Clicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new TypesPage())
            {
                Style = Application.Current.Resources[new ThemeChanger().Theme + "_Bar"] as Style
            }; // Load Types page

            Close();
        }

        private void BtnUsers_Clicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new Settings(this))
            {
                Style = Application.Current.Resources[new ThemeChanger().Theme + "_Bar"] as Style
            }; // Load Users page

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
            Btn_Events.Btn_Text = Resource.TitleEvents;
            Btn_DateSearch.Btn_Text = Resource.TitleDateSearch;
            Btn_CreateEvent.Btn_Text = Resource.TitleCreateEvent;
            Btn_Equip.Btn_Text = Resource.TitleEquipment;
            Btn_CreateEquip.Btn_Text = Resource.TitleCreateEquipment;
            Btn_Settings.Btn_Text = Resource.TitleSettings;
            Btn_About.Btn_Text = Resource.TitleAboutApp;
            Btn_Types.Btn_Text = Resource.TitleTypes;
            Btn_Users.Btn_Text = Resource.TitleUsers;
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
            Btn_CreateEvent.UpdateTheme();
            Btn_DateSearch.UpdateTheme();
            Btn_Equip.UpdateTheme();
            Btn_CreateEquip.UpdateTheme();
        }

        public async void Logout()
            => await Navigation.PopToRootAsync(true);

        // Close menu
        private void Close()
        {
            IsPresented = false;
        }

        private void ProfileImage_Tapped(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new ProfilePage(student.Data.User.Id, this))
            {
                Style = Application.Current.Resources[new ThemeChanger().Theme + "_Bar"] as Style
            }; // Load Profile page

            Close();
        }
    }
}