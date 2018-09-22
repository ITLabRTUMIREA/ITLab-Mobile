using Http_Post.Services;
using Http_Post.Res;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Http_Post.Pages
{
	public partial class ProfilePage : ContentPage
	{
        private UserView User;
        private HttpClient client = HttpClientFactory.HttpClient;
        private Menu menu;

        public ProfilePage (Guid id, Menu menu)
		{
            FindUser(id);
            this.menu = menu;
		}

        private void Init()
        {
            BindingContext = User;
            InitializeComponent();
            UpdateLanguage();
        }

        private async void FindUser(Guid id)
        {
            try
            {
                var response = await client.GetStringAsync($"user/{id}");
                var userRaw = JsonConvert.DeserializeObject<OneObjectResponse<UserView>>(response);

                if (userRaw.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {userRaw.StatusCode}");

                User = userRaw.Data;
                Init();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private void UpdateLanguage()
        {
            Title = Resource.Title_Profile;
            Lbl_Firstname.Text = Resource.ProfileLbl_Firstname;
            Lbl_Lastname.Text = Resource.ProfileLbl_Lastname;
            Lbl_PhoneNumber.Text = Resource.ProfileLbl_PhoneNumber;
            Btn_Save.Text = Resource.ProfileBtn_Save;
        }

        private async void Btn_Save_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool save = await DisplayAlert("", Resource.ADMIN_Sure, Resource.ADMIN_Yes, Resource.ADMIN_No);
                if (save)
                {
                    UserView userView = new UserView
                    {
                        FirstName = Entry_Firstname.Text,
                        LastName = Entry_Lastname.Text,
                        PhoneNumber = Entry_PhoneNumber.Text
                    };

                    var jsonContent = JsonConvert.SerializeObject(userView);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var result = await client.PutAsync("account", content);
                    string resultContent = await result.Content.ReadAsStringAsync();
                    var newUser = JsonConvert.DeserializeObject<OneObjectResponse<UserView>>(resultContent);

                    if (newUser.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                        throw new Exception($"Error: {newUser.StatusCode}");
                    else
                        await DisplayAlert("Message", Resource.ADMIN_Updated, "Ok");

                    menu.InitComponents();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }
    }
}