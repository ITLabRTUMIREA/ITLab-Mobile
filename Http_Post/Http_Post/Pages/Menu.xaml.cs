using Http_Post.Classes;
using Http_Post.Pages;
using Models.PublicAPI.Requests.Account;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.Login;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using Xamarin.Forms;

namespace Http_Post
{
	public partial class Menu : MasterDetailPage
    {
        private Localization localization = new Localization();

        private OneObjectResponse<LoginResponse> student;

        private string Name;
        private string LastName;

        public Menu ()
		{
			InitializeComponent ();
        }

        public Menu(OneObjectResponse<LoginResponse> student)
        {
            InitializeComponent();

            this.student = student;
            InitComponents();

            Detail = new NavigationPage(new EventPage(this.student));
            Close();
            //UpdateLanguage(); ------ Доделать
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
            Detail = new NavigationPage(new EventPage(student));
            Close();
        }

        private async void LogOut_Clicked (object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync(true); // Go to MainPage --- ( Login )
        }

        private void Settings_Clicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage (new Settings()); // Load Settings Page
            Close();
        }

        private async void AboutUs(object sender, EventArgs e)
        {
            await DisplayAlert("About", "Change with Resources!\nHelp us not to die\nMy name is Your Name\nBye", "Ok");
            Close();
        }

        private void UpdateLanguage()
        {
            // TODO: update all fields
            DisplayAlert("Default", "Default", "Default", "Default");
            //throw new NotImplementedException();
        }

        // Close menu
        private void Close()
        {
            IsPresented = false;
        }
    }
}