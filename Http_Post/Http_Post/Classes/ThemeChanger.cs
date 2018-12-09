using Http_Post.Res;
using System.Collections.Generic;

namespace Http_Post.Classes
{
    class ThemeChanger
    {
        public readonly List<string> ThemesForUser = new List<string>();

        private readonly string KEY = "theme";
        public readonly string[] themes =
        {
            Themes.Light.ToString(),
            Themes.Dark.ToString()
        };
        enum Themes
        {
            Light,
            Dark
        }

        public ThemeChanger()
        {
            ThemesForUser.Clear();
            ThemesForUser.Add(Resource.ThemeLight);
            ThemesForUser.Add(Resource.ThemeDark);
        }

        public void ChangeTheme(string theme)
        {
            if (theme.Equals(ThemesForUser[0]))
                Set(themes[0]);
            else if (theme.Equals(ThemesForUser[1]))
                Set(themes[1]);
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
                    return "Dark";
            }
        }
    }
}
