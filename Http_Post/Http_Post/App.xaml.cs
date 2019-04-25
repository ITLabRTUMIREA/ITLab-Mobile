using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Http_Post.Styles;
using Microsoft.AppCenter.Push;

namespace Http_Post
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            string str = Services.ReadDataFromSecret.GetValue("BaseAddress");

            if (!AppCenter.Configured)
            {
                Push.PushNotificationReceived += (sender, e) =>
                {
                    // Add the notification message and title to the message
                    var summary = $"Push notification received:" +
                                        $"\n\tNotification title: {e.Title}" +
                                        $"\n\tMessage: {e.Message}";

                    // If there is custom data associated with the notification,
                    // print the entries
                    if (e.CustomData != null)
                    {
                        summary += "\n\tCustom data:\n";
                        foreach (var key in e.CustomData.Keys)
                        {
                            summary += $"\t\t{key} : {e.CustomData[key]}\n";
                        }
                    }

                    // Send the notification summary to debug output
                    System.Diagnostics.Debug.WriteLine(summary);
                };
            }

            AppCenter.Start("ios=44f28903-3a0a-4349-ac68-193463cc7927;" +
                            "uwp=05d9ea9f-a247-4e8e-bc3f-26b2813cf42b;" +
                            "android=e1b02111-657f-44ee-82c6-21af51918f52",
                            typeof(Analytics), typeof(Crashes));
            AppCenter.Start("ios=44f28903-3a0a-4349-ac68-193463cc7927;" +
                            "uwp=05d9ea9f-a247-4e8e-bc3f-26b2813cf42b;" +
                            "android=e1b02111-657f-44ee-82c6-21af51918f52",
                            typeof(Push));

            Current.Resources.LoadTheme();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
