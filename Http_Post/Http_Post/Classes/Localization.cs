using Http_Post.Res;
using System.Collections.Generic;
using System.Globalization;

namespace Http_Post.Classes
{
    class Localization
    {
        private readonly string KEY = "language";

        public readonly string[] languages = { Language.Russian.ToString(), Language.English.ToString() };

        Dictionary<string, string> langDictionary = new Dictionary<string, string>();

        public Localization()
        {
            MakeDictionary();
            object name = "";
            if (App.Current.Properties.TryGetValue(KEY, out name))
            {
                // выполняем действия, если в словаре есть ключ "name"
                string lang = (string)name;

                Resource.Culture = new CultureInfo(lang);
            }
        }

        private void MakeDictionary()
        {
            langDictionary.Add("RUSSIAN", "ru-RU");
            langDictionary.Add("ENGLISH", "en-US");
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
