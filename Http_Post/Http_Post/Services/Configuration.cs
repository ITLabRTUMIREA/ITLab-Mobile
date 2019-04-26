using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace Http_Post.Services
{
    abstract public class Configuration : IConfiguration
    {
        public Lazy<string> BaseUrl { get; }

        public Lazy<Guid> AppCenterId { get; }

        public Configuration()
        {
            BaseUrl = new Lazy<string>(ReadBaseUrl);
            AppCenterId = new Lazy<Guid>(ReadAppCenterId);
        }

        protected abstract Guid ReadAppCenterId();

        private string ReadBaseUrl()
        {
            try
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(Configuration)).Assembly;
                Stream stream = assembly.GetManifestResourceStream("Http_Post.Res.secret.json");
                SecretData secretData;
                using (var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    secretData = JsonConvert.DeserializeObject<SecretData>(json);
                }
                return secretData.BaseAddress;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Can't read data from secret.json ---- " + ex.Message + " ---\n");
                return string.Empty;
            }
        }
    }

    public class SecretData
    {
        public string BaseAddress { get; set; }
        public Guid AppCenterId { get; set; }
    }
}
