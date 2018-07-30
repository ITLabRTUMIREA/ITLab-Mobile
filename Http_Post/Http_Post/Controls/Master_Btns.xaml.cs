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

        public Master_Btns ()
		{
			InitializeComponent ();

            UpdateTheme();
		}

        public void UpdateTheme()
        {
            var theme = new Classes.ThemeChanger();
            BackgroundColor = theme.ColorBG();
            Btn.BackgroundColor = theme.ColorBtn();
            Btn.TextColor = theme.ColorLbl();
        }

        // Only for 'Log out' button
        public void UpdateThemeRed()
        {
            BackgroundColor = new Classes.ThemeChanger().ColorBG();
            Btn.BackgroundColor = Color.FromHex("ff8080");
            Btn.TextColor = Color.Default;
        }
    }
}