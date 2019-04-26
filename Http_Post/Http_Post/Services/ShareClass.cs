using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Http_Post.Services
{
    class ShareClass
    {
        public async Task ShareText(string text)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = text,
                Title = "Share Text"
            });
        }

        public async Task ShareEvent(Guid eventId)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Uri = $"{DependencyService.Get<IConfiguration>().BaseUrl}events/{eventId}",
                Title = "Share Web Link"
            });
        }
    }
}
