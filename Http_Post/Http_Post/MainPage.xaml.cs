using Http_Post.Classes;
using Http_Post.Services;
using Models.PublicAPI.Requests.Account;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.Login;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;

namespace Http_Post
{
	public partial class MainPage : ContentPage
	{
        private readonly string host = "labworkback.azurewebsites.net"; // labworkback.azurewebsites.net // localhost
        private readonly string port = "80"; // 80 // 5000
        private readonly HttpClient client = HttpClientFactory.HttpClient;

        private void SPECIAL_DEBUG_FUNCTION()
        {
            text_login.Text = "test1@gmail.com"; // --------- Debug
            text_password.Text = "123456"; // --------------- Debug
            button_login.Focus(); // ------------------------ Debug
        }

        private string Login, PassWord;
        private Localization localization = new Localization();

		public MainPage()
		{
            TryLogin();

			InitializeComponent();

            UpdateLanguage();
            SetProgress(0.0);
            
            SPECIAL_DEBUG_FUNCTION();
        }

        private async void Button_login(object sender, EventArgs e)
        {
            SetProgress(0.0);

            text_error.TextColor = Color.Default; // Set Default Color
            text_error.Text = String.Empty; // Clear error field
            try
            {                
                Login = text_login.Text;
                PassWord = text_password.Text;
                if (!CheckForNull()) // if fields are empty -> user needs to enter them
                    return;

                SetProgress(0.4);
                text_error.Text = "Loading...\nPlease Wait...";

                AccountLoginRequest loginData = new AccountLoginRequest { Username = Login, Password = PassWord };

                // Convert data to a Json String
                var jsonContent = JsonConvert.SerializeObject(loginData);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                SetProgress(0.7);

                var result = await client.PostAsync("api/Authentication/login", content);
                string resultContent = await result.Content.ReadAsStringAsync();

                SetProgress(1.0);
                OneObjectResponse<LoginResponse> infoAboutStudent = JsonConvert.DeserializeObject<OneObjectResponse<LoginResponse>>(resultContent);
                Authorization(infoAboutStudent);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message + "\nCheck Internet connection");
            }
        }

        private async void SetProgress(double value)
        {
            if (Device.Idiom == TargetIdiom.Phone)
            
                progBar.IsVisible = false;
            else {
                await progBar.ProgressTo(value, 350, Easing.Linear);
            }
        }

        private async void Authorization (OneObjectResponse<LoginResponse> info)
        {
            if (info.StatusCode == Models.PublicAPI.Responses.ResponseStatusCode.OK) // if is OK
            {
                text_error.TextColor = Color.Green;
                text_error.Text = "Authorizated!";

                RememberToken(info);

                Menu menu = new Menu(info);
                NavigationPage.SetHasBackButton(menu, false); // Don't add back button
                NavigationPage.SetHasNavigationBar(menu, false); // Don't show "ITLab-Mobile" 'cause other titles seems confused
                await Navigation.PushAsync(menu);
                return;
            }

            if (info.StatusCode == Models.PublicAPI.Responses.ResponseStatusCode.WrongPassword) // if is ONLY password
            {
                text_password.Focus();
                ShowError("Wrong password");
                return;
            }

            if (info.StatusCode == Models.PublicAPI.Responses.ResponseStatusCode.UserNotFound) // if is User Wasn't found
            {
                text_login.Focus();
                ShowError("Check login or password");
                return;
            }

            text_login.Focus();
            ShowError(info.StatusCode.ToString());
        }

        private void ShowError(string error)
        {
            text_error.TextColor = Color.Red;
            text_error.Text = error;
        }

        private bool CheckForNull()
        {
            if (Login == null || Login == String.Empty) // if login is empty -> enter it
            {
                text_login.Focus();
                ShowError("Enter Login");
                return false;
            }

            if (PassWord == null || PassWord== String.Empty) // if password is empty -> enter it
            {
                text_password.Focus();
                ShowError("Enter Password");
                return false;
            }

            if (!CheckEmail())
                return false;
            
            return true;
        }

        private bool CheckEmail()
        {
            if (!Login.Contains("@")) // if no '@' -> reEnter
            {
                text_login.Focus();
                ShowError("Maybe you missed '@'");
                return false;
            }

            if (PassWord.Length < 6)
            {
                text_password.Focus();
                ShowError("Length is less than 6");
                return false;
            }

            return true;
        }

        private void UpdateLanguage()
        {
            text_label.Text = "Enter login and password";
            text_login.Placeholder = "Login";
            text_password.Placeholder = "Password";
            button_login.Text = "Login";
            text_error.Text = "";
        }

        private readonly string KEY = "refreshToken";
        private async void TryLogin()
        {
            if (App.Current.Properties.TryGetValue(KEY, out object value))
            {
                value = JsonConvert.SerializeObject(value);
                var content = new StringContent(value.ToString(), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/Authentication/refresh", content);

                string result = await response.Content.ReadAsStringAsync();

                OneObjectResponse<LoginResponse> infoAboutStudent = JsonConvert.DeserializeObject<OneObjectResponse<LoginResponse>>(result);
                if (infoAboutStudent.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                {
                    ShowError(infoAboutStudent.StatusCode.ToString());
                    return;
                }

                Authorization(infoAboutStudent);
            }
        }

        private void RememberToken(OneObjectResponse<LoginResponse> infoAboutStudent)
        {
            App.Current.Properties[KEY] = infoAboutStudent.Data.RefreshToken;
        }
    }
}
