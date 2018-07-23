﻿using Http_Post.Res;
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
        }

        public void Init()
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
            object name = "";
            if (App.Current.Properties.TryGetValue(KEY, out name))
            {
                // выполняем действия, если в словаре есть ключ "name"
                App.Current.Properties[KEY] = value;
            }
            else
            {
                // Добавить ключ "name" в словарь
                App.Current.Properties.Add(KEY, value);
            }
        }

        public string Dark_ColorBG = "#1a1a1a";
        public string Dark_ColorBtn = "#333333"; // #262626 (darker)
        public string Dark_ColorLbl = "#f2f2f2"; // #e6e6e6 (darker)
    }
}
