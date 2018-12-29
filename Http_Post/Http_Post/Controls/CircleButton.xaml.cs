using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CircleButton : ContentView
    {
        public event EventHandler BtnTapped;

        public CircleButton()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create("ImageSource",
            typeof(string),
            typeof(CircleButton),
            null,
            BindingMode.TwoWay,
            propertyChanged: ImageSourceChanged);

        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        static void ImageSourceChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            (bindableObject as CircleButton).imageCircle.Source = Images.ImageManager.GetSourceImage(newValue.ToString());
        }

        void btnCircle_Clicked(object sender, EventArgs e)
            => BtnTapped?.Invoke(this, EventArgs.Empty);
    }
}