using Http_Post.Classes;
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

        private string Name;
        private string LastName;
        HttpClient client;

        private string host = "labworkback.azurewebsites.net";
        private string port = "80";

        public Menu ()
		{
			InitializeComponent ();
        }

        public Menu(OneObjectResponse<LoginResponse> student)
        {
            InitializeComponent();

            InitComponents(student);

            //UpdateLanguage(); ------ Доделать
        }

        private void InitComponents(OneObjectResponse<LoginResponse> student)
        {
            Name = student.Data.FirstName;
            LastName = student.Data.LastName;

            label_name.Text = Name;
            label_surname.Text = LastName;

            client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", student.Data.Token);
        }

        private async void BtnEvents_Clicked(object sender, EventArgs e)
        {
            try
            {
                var response = await client.GetStringAsync($"http://{host}:{port}/api/event/");
                var events = JsonConvert.DeserializeObject<ListResponse<EventPresent>>(response);

                ListView listView = new ListView
                {
                    ItemsSource = events.Data.Select(ev => ev.Title ?? "no title")
                };

                stacklayout_Detail.Children.Add(listView);
            }
            catch (Exception ex)
            {
                stacklayout_Detail.Children.Add(new Label { Text = ex.Message });
            }
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

        // TODO: transfer to Settings.Page
        private void Lang_Clicked(object sender, EventArgs e)
        {
            AskForLanguage(localization);
        }

        private async void AboutUs(object sender, EventArgs e)
        {
            await DisplayAlert("About", "Change with Resources!\nHelp us not to die\nMy name is Your Name\nBye", "Ok");
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