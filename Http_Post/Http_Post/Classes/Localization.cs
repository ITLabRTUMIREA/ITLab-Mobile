using Http_Post.Res;
using Plugin.Settings;
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
            string culture = CrossSettings.Current.GetValueOrDefault(KEY, "");
            if (culture != "")
                return culture;
            else
            {
                CrossSettings.Current.AddOrUpdateValue(KEY, langDictionary["russian"]);
                return langDictionary["russian"];
            }
        }

        public void ChangeCulture ()
        {
            string culture = CrossSettings.Current.GetValueOrDefault(KEY, "");
            if (culture == langDictionary["russian"])
                CrossSettings.Current.AddOrUpdateValue(KEY, langDictionary["english"]);
            else
                CrossSettings.Current.AddOrUpdateValue(KEY, langDictionary["russian"]);
            SetCulture();
        }
    }
}
