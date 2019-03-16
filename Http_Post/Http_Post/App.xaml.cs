﻿using System;
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
            AppCenter.Start("ios={Your iOS App secret here};" +
                            "uwp={Your UWP App secret here};" +
                            "android={Your Android App secret here}",
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
