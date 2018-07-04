using Http_Post.Classes;
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
        private string Login, PassWord;
        private Localization localization = new Localization();


		public MainPage()
		{
			InitializeComponent();


            text_login.Text = "test@gmail.com"; // --------- Debug
            text_password.Text = "123456"; // -------------- Debug

            UpdateLanguage();
        }

        private async void Button_login(object sender, EventArgs e)
        {
            // TODO: progressing ring

            text_error.TextColor = Color.Default; // Set Default Color
            text_error.Text = String.Empty; // Clear error field
            try
            {
                HttpClient client = new HttpClient();
                
                Login = text_login.Text;
                PassWord = text_password.Text;
                if (!CheckForNull()) // if fields are empty -> user needs to enter them
                    return;

                text_error.Text = "Loading...\nPlease Wait...";

                AccountLoginRequest loginData = new AccountLoginRequest { Username = Login, Password = PassWord };

                // Convert data to a Json String
                var jsonContent = JsonConvert.SerializeObject(loginData);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var result = await client.PostAsync("https://labworkback.azurewebsites.net/api/Authentication/login", content);
                string resultContent = await result.Content.ReadAsStringAsync();

                //Label label = new Label { Text = $"{resultContent}" }; ------------------ Debug
                //stackLayout.Children.Add(label); ---------------------------------------- Debug

                OneObjectResponse<LoginResponse> infoAboutStudent = JsonConvert.DeserializeObject<OneObjectResponse<LoginResponse>>(resultContent);

                /*
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", infoAboutStudent.Data.Token);
                var response = await client.GetStringAsync("http://labworkback.azurewebsites.net:80/api/event/");
                Label l = new Label { Text = response };
                stackLayout.Children.Add(l);
                */

                Authorization(infoAboutStudent);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message + "\nCheck Internet connection");
            }
        }

        private async void Authorization (OneObjectResponse<LoginResponse> info)
        {
            if (info.StatusCode == Models.PublicAPI.Responses.ResponseStatusCode.OK) // if is OK
            {
                text_error.TextColor = Color.Green;
                text_error.Text = "Authorizated!";

                Menu menu = new Menu(info);
                NavigationPage.SetHasBackButton(menu, false); // Don't add back button
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

        private void Lang_Clicked(object sender, EventArgs e)
        {
            AskForLanguage(localization);
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
            text_label.Text = "Enter login and password";
            text_login.Placeholder = "Login";
            text_password.Placeholder = "Password";
            button_login.Text = "Login";
            text_error.Text = "";
        }
    }
}
