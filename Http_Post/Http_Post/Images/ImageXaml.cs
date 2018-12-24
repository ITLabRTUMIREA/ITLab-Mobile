using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Images
{
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }
            // TODO: get 'Black' or 'White' from ThemeManager
            var imageSource = ImageSource.FromResource("Http_Post.Images.Black." + Source + ".png");

            return imageSource;
        }
    }
}