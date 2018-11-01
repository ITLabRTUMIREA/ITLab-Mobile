using Http_Post.Classes;
using Http_Post.Pages;
using Xamarin.Forms;

namespace Http_Post
{
	public partial class Menu : TabbedPage
    {
        Localization localization = new Localization();

        public Menu()
        {
            InitializeComponent();

            AddSomeExtraPages();

            UpdateTheme();
        }

        void AddSomeExtraPages()
        {
            Children.Add(new Settings(this));
        }
        public void UpdateTheme()
        {
            var col = Application.Current.Resources;
            var th = new ThemeChanger().Theme;
            col["themeStack"] = col[th + "_Stack"];
            col["themeLabel"] = col[th + "_Lbl"];
            col["themeButton"] = col[th + "_Btn"];
            col["themeBar"] = col[th + "_Bar"];
        }

        public async void Logout()
            => await Navigation.PopToRootAsync();
    }
    
}