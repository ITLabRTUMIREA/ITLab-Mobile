using Http_Post.Services;
using Models.PublicAPI.Requests.Account;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.Login;
using Newtonsoft.Json;
using Plugin.Settings;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
	{
        readonly HttpClient client = HttpClientFactory.HttpClient;
        string Login, Password;

        public MainPage()
		{
            Init();
            TryLogin();
        }

        void Init()
        {
            InitializeComponent();
        }

        private async void Button_login(object sender, EventArgs e)
        {
            PageLoading(true);

            text_error.TextColor = Color.Default; // Set Default Color
            text_error.Text = String.Empty; // Clear error field
            try
            {                
                Login = text_login.Text.Trim();
                Password = text_password.Text;
                if (!CheckForNull()) // if fields are empty -> user needs to enter them
                {
                    PageLoading(false);
                    return;
                }

                text_error.Text = "Loading...\nPlease Wait...";

                AccountLoginRequest loginData = new AccountLoginRequest { Username = Login, Password = Password };

                // Convert data to a Json String
                var jsonContent = JsonConvert.SerializeObject(loginData);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var result = await client.PostAsync("Authentication/login", content);
                string resultContent = await result.Content.ReadAsStringAsync();

                PageLoading(false);

                OneObjectResponse<LoginResponse> infoAboutStudent = JsonConvert.DeserializeObject<OneObjectResponse<LoginResponse>>(resultContent);
                Authorization(infoAboutStudent, true);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message + "\nCheck Internet connection");
            }
        }

        private void Authorization (OneObjectResponse<LoginResponse> info, bool NeedToRender)
        {
            if (info.StatusCode == Models.PublicAPI.Responses.ResponseStatusCode.OK) // if is OK
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", info.Data.AccessToken);

                if (NeedToRender)
                {
                    text_error.TextColor = Color.Green;
                    text_error.Text = "Login successful!";
                }
                if (!NeedToRender)
                {
                    Init();
                    text_password.Text = ""; // clean password input
                }

                RememberToken(info); // remember refresh token
                new CurrentUserIdFactory().FirstSet(info.Data.User.Id, info.Data.Roles); // set user id and uesr roles in system

                Menu menu = new Menu();
                NavigationPage.SetHasNavigationBar(menu, false);
                Application.Current.MainPage = new NavigationPage(menu);
                //await Navigation.PushAsync(menu);
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

            ShowError(info.StatusCode.ToString());
            text_login.Focus();
        }

        void ShowError(string error)
        {
            text_error.TextColor = Color.Red;
            text_error.Text = error;
        }

        bool CheckForNull()
        {
            if (Login == null || Login == String.Empty) // if login is empty -> enter it
            {
                ShowError("Enter Login");
                text_login.Focus();
                return false;
            }

            if (Password == null || Password == String.Empty) // if password is empty -> enter it
            {
                ShowError("Enter Password");
                text_password.Focus();
                return false;
            }

            if (!CheckEmail())
                return false;
            
            return true;
        }

        bool CheckEmail()
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

        readonly string KEY = "refreshToken";
        async void TryLogin()
        {
            try
            {
                PageLoading(true);

                string token = CrossSettings.Current.GetValueOrDefault(KEY, "");
                var jsonContent = JsonConvert.SerializeObject(token);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("Authentication/refresh", content);
                string result = await response.Content.ReadAsStringAsync();

                PageLoading(false);

                OneObjectResponse<LoginResponse> infoAboutStudent = JsonConvert.DeserializeObject<OneObjectResponse<LoginResponse>>(result);
                if (infoAboutStudent.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                {
                    Init();
                    return;
                }

                Authorization(infoAboutStudent, false);
                Init();
            }
            catch (Exception ex)
            {
                Init();
                text_error.Text = ex.Message + '\n' + 
                    "Can't use refresh token to login";
            }
        }

        void PageLoading(bool y)
        {
            indicator.IsRunning = y;
            button_login.IsEnabled = !y;
            text_login.IsEnabled = !y;
            text_password.IsEnabled = !y;
        }

        void text_login_Completed(object sender, EventArgs e)
        {
            text_password.Focus();
        }

        void text_password_Completed(object sender, EventArgs e)
        {
            Button_login(button_login, EventArgs.Empty);
        }

        void RememberToken(OneObjectResponse<LoginResponse> infoAboutStudent)
        {
            CrossSettings.Current.AddOrUpdateValue(KEY, infoAboutStudent.Data.RefreshToken);
        }
    }
}