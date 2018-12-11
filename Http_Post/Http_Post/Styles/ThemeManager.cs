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
            if (App.Current.Properties.TryGetValue(KEY, out object name)) // if exists
                return (ResourceDictionary)name; // return this theme
            else // if not exist
            {
                App.Current.Properties.Add(KEY, Dark); // add dark theme
                return Dark; // return dark theme
            }
        }

        private static void SetCurrentTheme(ResourceDictionary res)
        {
            App.Current.Properties[KEY] = res;
        }

        // method should be used only once and only from App.OnStart()
        public static void LoadTheme(this Application app)
        {
            app.Resources.MergedDictionaries.Add(GetCurrentTheme());
        }

        public static void ChangeTheme(this Application app)
        {
            if (app.Resources.MergedDictionaries.Contains(Dark)) // if dark
            {
                // cahnge to light
                app.Resources.MergedDictionaries.Remove(Dark);
                app.Resources.MergedDictionaries.Add(Light);
                SetCurrentTheme(Light);
            }
            else // if light
            {
                // change to dark
                app.Resources.MergedDictionaries.Remove(Light);
                app.Resources.MergedDictionaries.Add(Dark);
                SetCurrentTheme(Dark);
            }
        }
    }
}
