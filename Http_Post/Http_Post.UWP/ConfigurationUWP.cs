using Http_Post.Services;
using Http_Post.UWP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(ConfigurationUWP))]
namespace Http_Post.UWP
{
    class ConfigurationUWP : Http_Post.Services.Configuration
    {
        protected override Guid ReadAppCenterId()
        {
            try
            {
                string json = System.IO.File.ReadAllText(@"Resources\secret.json");
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
