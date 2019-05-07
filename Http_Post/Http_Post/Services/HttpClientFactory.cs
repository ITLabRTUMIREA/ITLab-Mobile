using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.Login;
using Newtonsoft.Json;
using Plugin.Settings;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Http_Post.Services
{
    class HttpClientFactory
    {
        public static HttpClient HttpClient { get; } = CreateHttpClient();
        private static HttpClient CreateHttpClient()
        {
            var configuration = DependencyService.Get<IConfiguration>();
            var baseAddr = configuration.BaseUrl.Value + "api/";

            var httpClient = new HttpClient(new AutoRefreshHttpHandler(baseAddr));
            httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent());
            httpClient.BaseAddress = new Uri(baseAddr);

            return httpClient;
        }

        private static string UserAgent()
        {
            //TODO: Get correct app version
            var version = $"1.0.0";
            return $"Xamarin.{Device.RuntimePlatform}/{version}";
        }
    }

    class AutoRefreshHttpHandler : HttpClientHandler
    {
        private readonly string baseUri;

        public AutoRefreshHttpHandler(string baseUri)
        {
            this.baseUri = baseUri;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CrossSettings.Current.GetValueOrDefault("access_token", ""));
            var response = await base.SendAsync(request, cancellationToken);
            var content = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ResponseBase>(content);
            if (responseObject.StatusCode == ResponseStatusCode.Unauthorized)
            {
                var refreshResponse = await base.SendAsync(HttpClientExtensions.RefreshTokenMessage(baseUri), cancellationToken);
                await HttpClientExtensions.HandleRefreshResponse(refreshResponse);
                return await SendAsync(request, cancellationToken);
            }
            return response;
        }
    }

    //readonly string KEY = "refreshToken";
    static class HttpClientExtensions
    {
        const string KEY = "refresh_token";
        public static async Task<OneObjectResponse<LoginResponse>> RefreshToken(this HttpClient httpClient)
        {
            var response = await httpClient.SendAsync(RefreshTokenMessage(httpClient.BaseAddress.ToString()));
            var handled = await HandleRefreshResponse(response);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", handled.Data.AccessToken);
            return handled;
        }
        public static async Task<OneObjectResponse<LoginResponse>> HandleRefreshResponse(HttpResponseMessage responseMessage)
        {
            string result = await responseMessage.Content.ReadAsStringAsync();
            var loginResponse = JsonConvert.DeserializeObject<OneObjectResponse<LoginResponse>>(result);
            if (loginResponse.StatusCode == ResponseStatusCode.OK)
            {
                CrossSettings.Current.AddOrUpdateValue(KEY, loginResponse.Data.RefreshToken);
                CrossSettings.Current.AddOrUpdateValue("access_token", loginResponse.Data.AccessToken);
            }
            return loginResponse;
        }
        public static HttpRequestMessage RefreshTokenMessage(string baseUri)
        {
            string token = CrossSettings.Current.GetValueOrDefault(KEY, "");
            var jsonContent = JsonConvert.SerializeObject(token);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = content,
                RequestUri = new Uri($"{baseUri}Authentication/refresh")
            };
        }
    }
}