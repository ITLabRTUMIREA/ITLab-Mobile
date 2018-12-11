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

        public static ResourceDictionary GetCurrentTheme()
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
        /*public static void LoadTheme(this ResourceDictionary res)
        {
            res.MergedDictionaries.Add(GetCurrentTheme());
        }

        public static void ChangeTheme(this ResourceDictionary res)
        {
            if (res.MergedDictionaries.Contains(Dark)) // if dark
            {
                res.MergedDictionaries.Remove(Dark);
                res.MergedDictionaries.Add(Light);
                SetCurrentTheme(Light);
            }
            else
            {
                res.MergedDictionaries.Remove(Light);
                res.MergedDictionaries.Add(Dark);
                SetCurrentTheme(Dark);
            }
        }
        */
        public static void ChangeTheme()
        {
            if (GetCurrentTheme() == Dark)

                SetCurrentTheme(Light);
            else
                SetCurrentTheme(Dark);
        }
    }
}
