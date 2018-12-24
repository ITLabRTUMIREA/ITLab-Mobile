using Xamarin.Forms;

namespace Http_Post.Images
{
    static class ImageManager
    {
        public static ImageSource GetSource(string Source)
        {
            // TODO: get 'Black' or 'White' from ThemeManager
            return ImageSource.FromResource("Http_Post.Images.Black." + Source + ".png");
        }
    }
}