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
        private readonly HttpClient client = HttpClientFactory.HttpClient;

        private void SPECIAL_DEBUG_FUNCTION()
        {
            text_login.Text = "test@gmail.com"; // --------- Debug
            text_password.Text = "123456"; // --------------- Debug
            button_login.Focus(); // ------------------------ Debug
        }

        private string Login, Password;

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
                Password = text_password.Text;
                if (!CheckForNull()) // if fields are empty -> user needs to enter them
                    return;

                SetProgress(0.4d);
                text_error.Text = "Loading...\nPlease Wait...";

                AccountLoginRequest loginData = new AccountLoginRequest { Username = Login, Password = Password };

                // Convert data to a Json String
                var jsonContent = JsonConvert.SerializeObject(loginData);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                SetProgress(0.7d);

                var result = await client.PostAsync("Authentication/login", content);
                string resultContent = await result.Content.ReadAsStringAsync();

                SetProgress(1.0d);
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
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", info.Data.AccessToken);

                text_error.TextColor = Color.Green;
                text_error.Text = "Authorizated!";

                RememberToken(info); // remember refresh token
                new CurrentUserIdFactory().FirstSet(info.Data.User.Id, info.Data.Roles); // set user id and uesr roles in system

                Menu menu = new Menu();
                NavigationPage.SetHasBackButton(menu, false); // Don't add back button
                /*Application.Current.MainPage = new NavigationPage(new Menu {
                    BarBackgroundColor = Color.FromHex("009688"),
                    BarTextColor = Color.White
                });*/
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

            if (Password == null || Password == String.Empty) // if password is empty -> enter it
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

            if (Password.Length < 6)
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
            try
            {
                if (App.Current.Properties.TryGetValue(KEY, out object value))
                {
                    value = JsonConvert.SerializeObject(value);
                    var content = new StringContent(value.ToString(), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("Authentication/refresh", content);

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
            catch (Exception ex)
            {
                text_error.Text = ex.Message + '\n' + 
                    "Can't use refresh token to login";
            }
        }

        private void text_login_Completed(object sender, EventArgs e)
        {
            text_password.Focus();
        }

        private void text_password_Completed(object sender, EventArgs e)
        {
            Button_login(button_login, EventArgs.Empty);
        }

        private void RememberToken(OneObjectResponse<LoginResponse> infoAboutStudent)
        {
            App.Current.Properties[KEY] = infoAboutStudent.Data.RefreshToken;
        }
    }
}
