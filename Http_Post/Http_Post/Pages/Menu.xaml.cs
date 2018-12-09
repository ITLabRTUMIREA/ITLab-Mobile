using Http_Post.Classes;
using Http_Post.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Menu : TabbedPage
    {
        Localization localization = new Localization();

        public Menu()
        {
            InitializeComponent();

            AddPages(false);

            UpdateTheme();
        }

        void AddPages(bool isUpdating)
        {
            Children.Add(new EventPage());
            Children[0].Icon = "News.png";
            Children.Add(new EquipmentPage());
            Children[1].Icon = "Repair.png";
            Children.Add(new TypesPage());
            Children[2].Icon = "TwoLinesHorizontal.png";
            Children.Add(new Settings(this));
            Children[3].Icon = "SettingGear.png";
            if (isUpdating)
                CurrentPage = Children[Children.Count - 1];
        }

        public void UpdatePages()
        {
            Children.Clear();
            AddPages(true);
        }

        public void UpdateTheme()
        {
            var col = Application.Current.Resources;
            var th = new ThemeChanger().Theme;
            col["themeStack"] = col[th + "_Stack"];
            col["themeLabel"] = col[th + "_Lbl"];
            col["themeButton"] = col[th + "_Btn"];
            col["themeBar"] = col[th + "_Bar"];
            col["themeFrame"] = col[th + "_Frame"];
            col["themeTab"] = col[th + "_Tab"];
            //Style = col[th + "_Tab"] as Style;
        }

        public async void Logout()
            => await Navigation.PopToRootAsync();

        protected override bool OnBackButtonPressed()
        {
            //return base.OnBackButtonPressed();
            return true; // Android bug fix - exit to login page
        }
    }
    
}