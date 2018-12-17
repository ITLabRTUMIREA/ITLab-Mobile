using Http_Post.Res;
using System.Collections.Generic;
using System.Globalization;

namespace Http_Post.Classes
{
    class Localization
    {
        private readonly string KEY = "language";

        public readonly string[] languages = { Language.Russian.ToString(), Language.English.ToString() };

        private static readonly Dictionary<string, string> langDictionary = new Dictionary<string, string>()
        {
            { "RUSSIAN","ru-RU" },
            { "ENGLISH", "en-US" }
        };

        public Localization()
        {
            if (App.Current.Properties.TryGetValue(KEY, out object name))
            {
                // выполняем действия, если в словаре есть ключ "name"
                string lang = (string)name;

                Resource.Culture = new CultureInfo(lang);
            }
            else
            {
                App.Current.Properties[KEY] = langDictionary["ENGLISH"];
                Resource.Culture = new CultureInfo(langDictionary["ENGLISH"]);
            }
        }

        public void ChangeCulture (string lang)
        {
            if (lang.Equals(Language.Russian.ToString().ToUpper()))
            {
                Resource.Culture = new CultureInfo("ru-RU");
            }

            else if (lang.Equals(Language.English.ToString().ToUpper()))
            {
                Resource.Culture = new CultureInfo("en-US");
            }

            RememberCulture(lang);
        }

        private void RememberCulture(string language)
        {
            language = langDictionary[language];
            object name = "";
            if (App.Current.Properties.TryGetValue(KEY, out name))
            {
                // выполняем действия, если в словаре есть ключ "name"
                App.Current.Properties[KEY] = language;
            }
            else
            {
                // Добавить ключ "name" в словарь
                App.Current.Properties.Add(KEY, language);
            }
        }

        enum Language
        {
            Russian,
            English,
        }
    }
}
