using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Http_Post.Styles
{
    static class ThemeManager
    {
        private static readonly string KEY = "theme";

        private static ResourceDictionary Dark { get; } = new DarkStyle();
        private static ResourceDictionary Light { get; } = new LightStyle();

        private static ResourceDictionary GetCurrentTheme()
        {
            string name = CrossSettings.Current.GetValueOrDefault(KEY, "");
            if (name != "")
            {
                if (name == nameof(Light))
                    return Light;
                else
                    return Dark;
            }
            else
            {
                CrossSettings.Current.AddOrUpdateValue(KEY, nameof(Light));
                return Light;
            }
        }

        private static void SetCurrentTheme(string name)
        {
            CrossSettings.Current.AddOrUpdateValue(KEY, name);
        }

        // method should be used only once and only from App.OnStart()
        public static void LoadTheme(this ResourceDictionary res)
        {
            res.Add(GetCurrentTheme());
        }

        public static void ChangeTheme(this ResourceDictionary app)
        {
            app.Clear();
            if (GetCurrentTheme() == Light)
            {
                app.Add(Dark);
                SetCurrentTheme(nameof(Dark));
            }
            else
            {
                app.Add(Light);
                SetCurrentTheme(nameof(Light));
            }
        }
    }
}
