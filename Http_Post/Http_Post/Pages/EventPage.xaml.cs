using Http_Post.Classes;
using Http_Post.Controls;
using Http_Post.Extensions.Responses.Event;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.Login;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Http_Post.Pages
{
	public partial class EventPage : ContentPage
	{
        HttpClient client;

        private string host = "labworkback.azurewebsites.net";
        private string port = "80";

        private OneObjectResponse<LoginResponse> student;
        private ListResponse<CompactEventViewExtended> events;

        public EventPage ()
		{
			InitializeComponent ();
        }

        public EventPage(OneObjectResponse<LoginResponse> student)
        {
            InitializeComponent();
            Title = Res.Resource.Title_Event;

            this.student = student;
            GetEvents();

            UpdateTheme();
        }

        private void UpdateTheme()
        {
            ThemeChanger themeChanger = new ThemeChanger();
            if (App.Current.Properties.TryGetValue("theme", out object name))
            {
                if (App.Current.Properties["theme"].Equals(themeChanger.themes[1]))
                    stacklayout.BackgroundColor = Color.FromHex(themeChanger.Dark_ColorBG);
            }
        }

        private async void GetEvents()
        {
            try
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", student.Data.Token);

                var response = await client.GetStringAsync($"http://{host}:{port}/api/event/");
                events = JsonConvert.DeserializeObject<ListResponse<CompactEventViewExtended>>(response);

                ShowEvents();
            }
            catch (Exception ex)
            {
                stacklayout.Children.Add(new Label { Text = ex.Message });
            }
        }

        private async void ShowEvents()
        {
            try
            {
                listView.ItemsSource = events.Data;
            }
            catch (Exception ex)
            {
                stacklayout.Children.Add(new Label { Text = ex.Message });
            }
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (CompactEventView)e.SelectedItem;

            // Open new page
        }
    }
}