using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Http_Post.Services
{
    static class ReadDataFromSecret
    {
        public static string GetValue(string PropertyName)
        {
            try
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(ReadDataFromSecret)).Assembly;
                Stream stream = assembly.GetManifestResourceStream("Http_Post.Res.secret.json");
                SecretData secretData;
                using (var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    secretData = JsonConvert.DeserializeObject<SecretData>(json);
                }
                switch (PropertyName)
                {
                    case "BaseAddress":
                        return secretData.BaseAddress;
                    case "AppCenterAndroid":
                        return secretData.AppCenterAndroid;
                    case "AppCenteriOS":
                        return secretData.AppCenteriOS;
                    case "AppCenterUWP":
                        return secretData.AppCenterUWP;
                    default:
                        return null;
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Can't read data from secret.json ---- " + ex.Message + " ---\n");
                return null;
            }
        }
    }
    
    class SecretData
    {
        public string BaseAddress { get; set; }
        public string AppCenterAndroid { get; set; }
        public string AppCenteriOS { get; set; }
        public string AppCenterUWP { get; set; }
    }
}
