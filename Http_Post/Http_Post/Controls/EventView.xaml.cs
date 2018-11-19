﻿using System.Linq;
using Xamarin.Forms;

namespace Http_Post.Controls
{
	public partial class EventView : ViewCell
	{
        public EventView ()
        {
            InitializeComponent();

            UpdateTheme();
        }

        void UpdateTheme()
        {
            var th = new Classes.ThemeChanger().Theme;
            var col = Application.Current.Resources;
            col["themeStack"] = col[th + "_Stack"];
            col["themeLabel"] = col[th + "_Lbl"];

            lblPercent.BackgroundColor = progBar.ProgressColor;
        }
    }
}