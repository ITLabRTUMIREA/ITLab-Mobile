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
        }

        void AddPages(bool isUpdating)
        {
            Children.Add(new NotificationPage());
            Children[0].Icon = "Notification.png";
            Children.Add(new EventPage());
            Children[1].Icon = "News.png";
            Children.Add(new EquipmentPage());
            Children[2].Icon = "Repair.png";
            Children.Add(new TypesPage());
            Children[3].Icon = "TwoLinesHorizontal.png";
            Children.Add(new Settings(this));
            Children[4].Icon = "SettingGear.png";
            if (isUpdating)
                CurrentPage = Children[Children.Count - 1];
        }

        public void UpdatePages()
        {
            Children.Clear();
            AddPages(true);
        }

        public async void Logout()
        {
            App.Current.Properties["refreshToken"] = "";
            await Navigation.PopToRootAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            //return base.OnBackButtonPressed();
            return true; // Android bug fix - exit to login page
        }
    }
    
}