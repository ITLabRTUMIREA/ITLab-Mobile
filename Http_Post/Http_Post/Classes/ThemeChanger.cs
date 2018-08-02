using Http_Post.Res;
using System;
using System.Collections.Generic;
using System.Text;

namespace Http_Post.Classes
{
    class ThemeChanger
    {
        public readonly List<string> ThemesForUser = new List<string>();

        private readonly string KEY = "theme";
        public readonly string[] themes =
        {
            Themes.Default.ToString(),
            Themes.Dark.ToString(),
            Themes.Light.ToString(),
            Themes.Rainbow.ToString()
        };
        enum Themes
        {
            Default,
            Dark,
            Light,
            Rainbow
        }

        public ThemeChanger()
        {
            ThemesForUser.Clear();
            ThemesForUser.Add(Resource.ThemeDefault);
            ThemesForUser.Add(Resource.ThemeDark);
            ThemesForUser.Add(Resource.ThemeLight);
            ThemesForUser.Add(Resource.ThemeRainbow);
        }

        public void ChangeTheme(string theme)
        {
            if (theme.Equals(ThemesForUser[0]))
                Set(themes[0]);
            else if (theme.Equals(ThemesForUser[1]))
                Set(themes[1]);
            else if (theme.Equals(ThemesForUser[2]))
                Set(themes[2]);
            else if (theme.Equals(ThemesForUser[3]))
                Set(themes[3]);
        }

        private void Set(string value)
        {
            if (App.Current.Properties.TryGetValue(KEY, out object name))
                // выполняем действия, если в словаре есть ключ "name"
                App.Current.Properties[KEY] = value;
            else
                // Добавить ключ "name" в словарь
                App.Current.Properties.Add(KEY, value);
        }

        public string Theme
        {
            get
            {
                if (App.Current.Properties.TryGetValue(KEY, out object name))
                    return name.ToString();
                else
                    return "Default";
            }
        }
    }
}
