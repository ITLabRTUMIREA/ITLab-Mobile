using System;
using System.IO;
using Http_Post.Droid;
using Http_Post.Services;
using Newtonsoft.Json;
using Xamarin.Forms;

[assembly: Dependency(typeof(ConfigurationAndroid))]
namespace Http_Post.Droid
{
    class ConfigurationAndroid : Http_Post.Services.Configuration
    {
        protected override Guid ReadAppCenterId()
        {
            try
            {
                using(var reader = new StreamReader(Android.App.Application.Context.Assets.Open("secret.json")))
                {
                    string json = reader.ReadToEnd();
                    SecretData secretData = JsonConvert.DeserializeObject<SecretData>(json);
                    return secretData.AppCenterId;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Can't read data from secret.json ---- " + ex.Message + " ---\n");
                throw ex;
            }
        }
    }
}