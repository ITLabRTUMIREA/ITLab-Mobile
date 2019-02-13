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
	public partial class ButtonsArrows : ContentView
	{
        public event EventHandler tapped;
        public string ImageLeft
        {
            get { return imageLeft.Source.ToString(); }
            set { imageLeft.Source = value; }
        }
        public string ImageRight
        {
            get { return imageRight.Source.ToString(); }
            set { imageRight.Source = value; }
        }
        public Color FrameColor
        {
            get { return frame.BorderColor; }
            set { frame.BorderColor = value; }
        }

        public ButtonsArrows ()
		{
			InitializeComponent ();
		}

        void TapGestureRecognizer_Tapped(object sender, EventArgs e)
            => tapped?.Invoke(sender, e);
	}
}