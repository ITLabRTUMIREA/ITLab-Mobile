using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Http_Post.Controls
{
	public partial class ShiftsView : ViewCell
	{
		public ShiftsView ()
		{
			InitializeComponent ();

            UpdateTheme();
        }

        private void UpdateTheme()
        {
            var th = new Classes.ThemeChanger().Theme;
            var col = Application.Current.Resources;
            col["themeStack"] = col[th + "_Stack"];
            col["themeLabel"] = col[th + "_Lbl"];
        }
    }
}