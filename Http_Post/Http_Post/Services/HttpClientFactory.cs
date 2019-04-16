using System;
using System.IO;
using System.Net.Http;
using Xamarin.Forms;

namespace Http_Post.Services
{
    class HttpClientFactory
    {
        public static HttpClient HttpClient { get; } = CreateHttpClient();
        private static HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent());
            httpClient.BaseAddress = new Uri(Res.secret.baseAddress + "api/");
            return httpClient;
        }

        private static string UserAgent()
        {
            //TODO: Get correct app version
            var version = $"1.0.0";
            return $"Xamarin.{Device.RuntimePlatform}/{version}";
        }
    }
}