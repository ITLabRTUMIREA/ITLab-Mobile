using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Http_Post.Styles;

namespace Http_Post
{
    public partial class App : Application
	{
        public App ()
		{
			InitializeComponent();

            MainPage = new MainPage();
		}

		protected override void OnStart ()
		{
            // Handle when your app starts
            AppCenter.Start("ios=44f28903-3a0a-4349-ac68-193463cc7927;" +
                            "uwp=05d9ea9f-a247-4e8e-bc3f-26b2813cf42b;" +
                            "android=e1b02111-657f-44ee-82c6-21af51918f52",
                            typeof(Analytics), typeof(Crashes));

            Current.Resources.LoadTheme();
        }

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
