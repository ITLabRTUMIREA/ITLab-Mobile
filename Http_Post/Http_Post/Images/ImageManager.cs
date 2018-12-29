using Plugin.Settings;
using Xamarin.Forms;

namespace Http_Post.Images
{
    static class ImageManager
    {
        public static ImageSource GetSourceImage(string Source)
        {
            // light theme -> dark icons (that's why in light folder locate dark icons)
            // TODO: get 'Black' or 'White' from ThemeManager
            string theme = CrossSettings.Current.GetValueOrDefault("theme", null);
            return ImageSource.FromResource($"Http_Post.Images.{theme}.{Source}.png");
        }
    }
}