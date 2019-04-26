using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Foundation;
using Http_Post.iOS;
using Http_Post.Services;
using Newtonsoft.Json;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(ConfigurationiOS))]
namespace Http_Post.iOS
{
    class ConfigurationiOS : Http_Post.Services.Configuration
    {
        protected override Guid ReadAppCenterId()
        {
            try
            {
                var json = File.ReadAllText("Assets/secret.json");
                SecretData secretData = JsonConvert.DeserializeObject<SecretData>(json);
                return secretData.AppCenterId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Can't read data from secret.json ---- " + ex.Message + " ---\n");
                throw ex;
            }
        }
    }
}