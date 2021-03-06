﻿using Models.PublicAPI.Requests.Account;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.Login;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;

namespace Http_Post
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

        private async void Button_login(object sender, EventArgs e)
        {
            text_error.Text = String.Empty; // Clear error field
            try
            {
                HttpClient client = new HttpClient();
                // Create new User with Login and Password
                if (!Check())
                    return;
                AccountLoginRequest loginData = new AccountLoginRequest { Username = text_login.Text, Password = text_password.Text };

                // Convert data to a Json String
                var jsonContent = JsonConvert.SerializeObject(loginData);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var result = await client.PostAsync("https://labworkback.azurewebsites.net/api/Authentication/login", content);
                string resultContent = await result.Content.ReadAsStringAsync();

                //Label label = new Label { Text = $"{resultContent}" }; ------------------ Debug
                //stackLayout.Children.Add(label); ---------------------------------------- Debug

                OneObjectResponse<LoginResponse> infoAboutStudent = JsonConvert.DeserializeObject<OneObjectResponse<LoginResponse>>(resultContent);
                Authorization(infoAboutStudent);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void Authorization (OneObjectResponse<LoginResponse> info)
        {
            if (info.StatusCode == Models.PublicAPI.Responses.ResponseStatusCode.OK) // if is OK
            {
                text_error.TextColor = Color.Green;
                text_error.Text = "МАКС ЧЁ ДЕЛАТЬ Я АВТОРИЗОВАЛСЯ";
                text_error.Text += "\nЗагрузить новую страницу?\n";
                text_error.Text += "Authorizated!";
                return;
            }

            if (info.StatusCode == Models.PublicAPI.Responses.ResponseStatusCode.WrongPassword) // if is ONLY password
                text_password.Focus();
            else
                text_login.Focus();


            //ShowError(info.StatusCode.ToString());
        }

        private void ShowError(string error)
        {
            text_error.TextColor = Color.Red;
            text_error.Text = error;

        }

        private bool Check()
        {
            if (text_login.Text == null || text_login.Text == String.Empty)
            {
                text_login.Focus();
                ShowError("Wrong login");
                return false;
            }

            if (text_password.Text == null || text_password.Text == String.Empty)
            {
                text_password.Focus();
                ShowError("Wrong password");
                return false;
            }

            return true;
        }
    }
}
