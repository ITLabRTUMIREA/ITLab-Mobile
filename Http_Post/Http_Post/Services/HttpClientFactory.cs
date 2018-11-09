using System;
using System.Net.Http;
using Xamarin.Forms;

namespace Http_Post.Services
{
    class HttpClientFactory
    {
        //private const string url = "http://localhost:5000/api/";
        private const string url = "http://dev.rtuitlab.ru/api/";

        public static HttpClient HttpClient { get; } = CreateHttpClient();
        private static HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent());
            httpClient.BaseAddress = new Uri(url);
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
