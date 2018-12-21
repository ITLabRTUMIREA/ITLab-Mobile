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
                return (ResourceDictionary)name; 
            else
            {
                App.Current.Properties[KEY] = Light;
                return Light;
            }
        }

        private static void SetCurrentTheme(ResourceDictionary res)
        {
            App.Current.Properties[KEY] = res;
        }

        // method should be used only once and only from App.OnStart()
        public static void LoadTheme(this ResourceDictionary res)
        {
            res.MergedDictionaries.Add(GetCurrentTheme());
        }

        public static void ChangeTheme(this ResourceDictionary app)
        {
            if (app.MergedDictionaries.Contains(Dark)) // if dark
            {
                // cahnge to light
                app.MergedDictionaries.Remove(Dark);
                app.MergedDictionaries.Add(Light);
                SetCurrentTheme(Light);
            }
            else
            {
                // change to dark
                app.MergedDictionaries.Remove(Light);
                app.MergedDictionaries.Add(Dark);
                SetCurrentTheme(Dark);
            }

            //if (GetCurrentTheme() == Dark)
            //{
            //    app.Clear();
            //    SetCurrentTheme(Light);
            //    app.LoadTheme();
            //}
            //else
            //{
            //    app.Clear();
            //    SetCurrentTheme(Dark);
            //    app.LoadTheme();
            //}
        }
        /*
        public static void ChangeTheme()
        {
            if (GetCurrentTheme() == Dark)

                SetCurrentTheme(Light);
            else
                SetCurrentTheme(Dark);
        }
        */
    }
}
