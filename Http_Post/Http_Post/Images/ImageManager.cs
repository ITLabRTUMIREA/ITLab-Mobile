using Plugin.Settings;
using Xamarin.Forms;

namespace Http_Post.Images
{
    static class ImageManager
    {
        public static string GetSourceImage(string Source)
        {
            // light theme -> dark icons (that's why in light folder locate dark icons)
            if (Source == null)
            {
                return null;
            }
            string theme = CrossSettings.Current.GetValueOrDefault("theme", null);
            //var imageSource = ImageSource.FromResource($"resource://Http_Post.Images.{theme}.{Source}.svg");
            return $"resource://Http_Post.Images.{theme}.{Source}.svg";
        }
    }
}