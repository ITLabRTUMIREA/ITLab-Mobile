﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xamarin.Forms;

namespace Http_Post.Services
{
    class HttpClientFactory
    {
        private const string url = "https://itlabdevelop.azurewebsites.net/api/";

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