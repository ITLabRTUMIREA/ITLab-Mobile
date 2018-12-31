using Plugin.Settings;
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