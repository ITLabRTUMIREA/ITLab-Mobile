using System.Linq;
using Xamarin.Forms;

namespace Http_Post.Controls
{
	public partial class EventView : ViewCell
	{
        public EventView ()
        {
            InitializeComponent();

            UpdateTheme();
            test();
        }

        void test()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    {
                        grid
                            .Children
                            .Select((item, index) => {
                                item.IsVisible = index < 2;
                                return item;
                        }).ToList();
                        /*int counter = 0;
                        foreach(View view in grid.Children)
                        {
                            if (counter < 2)
                                continue;

                            view.IsVisible = false;
                        }*/
                    }
                    break;
            }
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