﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;


using Xamarin.Forms;

namespace Http_Post
{
    public partial class App : Application
	{
        public App ()
		{
			InitializeComponent();

            //MainPage = new Http_Post.MainPage();
            MainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = Color.FromHex("009688")
            };
            //MainPage = new MainPage();
		}

		protected override void OnStart ()
		{
            // Handle when your app starts
            AppCenter.Start("ios=44f28903-3a0a-4349-ac68-193463cc7927;" +
                            "uwp={Your UWP App secret here};" +
                            "android={Your Android App secret here}",
                            typeof(Analytics), typeof(Crashes));

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
