using Http_Post.Res;
using System.Collections.Generic;
using System.Globalization;

namespace Http_Post.Classes
{
    class Localization
    {
        private readonly string KEY = "language";

        private static readonly Dictionary<string, string> langDictionary = new Dictionary<string, string>()
        {
            { "russian","ru-RU" },
            { "english", "en-US" }
        };

        public Localization()
        {
            SetCulture();
        }

        void SetCulture()
        {
            Resource.Culture = new CultureInfo(GetCultureProperties());
        }

        string GetCultureProperties ()
        {
            if (App.Current.Properties.TryGetValue(KEY, out object name))
            {
                // выполняем действия, если в словаре есть ключ "name"
                return (string)name;
            }
            else
            {
                App.Current.Properties[KEY] = langDictionary["russian"];
                return langDictionary["russian"];
            }
        }

        public void ChangeCulture ()
        {
            string name = App.Current.Properties[KEY].ToString();
            if (name == langDictionary["russian"])
                App.Current.Properties[KEY] = langDictionary["english"];
            else
                App.Current.Properties[KEY] = langDictionary["russian"];

            SetCulture();
        }
    }
}
