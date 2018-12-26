using Xamarin.Forms;

namespace Http_Post.Images
{
    static class ImageManager
    {
        public static ImageSource GetSourceImage(string Source)
        {
            // light theme -> dark icons (that's why in light folder locate dark icons)
            // TODO: get 'Black' or 'White' from ThemeManager
            return ImageSource.FromResource($"Http_Post.Images.Light.{Source}.png");
        }
    }
}