using System;
using Xamarin.Forms;

namespace Http_Post.Controls
{
	public partial class Master_Btns : ContentView
	{
        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create("ImageSource",
            typeof(string),
            typeof(Master_Btns),
            null,
            BindingMode.TwoWay,
            propertyChanged:ImageSourceChanged);

        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        private static void ImageSourceChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            (bindableObject as Master_Btns).Icon.Source = Convert.ToString(newValue);
        }

        public static readonly BindableProperty BtnTextProperty = BindableProperty.Create("Btn_Text",
            typeof(string),
            typeof(Master_Btns),
            null,
            BindingMode.TwoWay,
            propertyChanged: BtnTextChanged);

        public string Btn_Text
        {
            get { return (string)GetValue(BtnTextProperty); }
            set { SetValue(BtnTextProperty, value); }
        }

        private static void BtnTextChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            (bindableObject as Master_Btns).Btn.Text = Convert.ToString(newValue);
        }

        public event EventHandler Clicked;

        private void Btn_Clicked (object sender, EventArgs e)
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }

        public static readonly BindableProperty BackProperty = BindableProperty.Create("Back",
           typeof(string),
           typeof(Master_Btns),
           null,
           BindingMode.TwoWay,
           propertyChanged: BackChanged);

        private static void BackChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            (bindableObject as Master_Btns).Btn.BackgroundColor = Color.FromHex(Convert.ToString(newValue));
        }

        public string Back
        {
            get { return (string)GetValue(BackProperty); }
            set { SetValue(BackProperty, value); }
        }


        public Master_Btns ()
		{
			InitializeComponent ();
		}
	}
}