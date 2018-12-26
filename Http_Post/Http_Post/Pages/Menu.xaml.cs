using Http_Post.Classes;
using Http_Post.Pages;
using Plugin.Settings;
using System.Threading.Tasks;
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
            Children[0].Icon = "Notifications.png";
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
            CrossSettings.Current.AddOrUpdateValue("refreshToken", "");
            await Navigation.PopToRootAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            //Bug - can't save App.Current.Properties
            if (Device.RuntimePlatform == Device.Android)
            {
                DisplayAlert("", "�� ������� ��� ������ �����?", "��", "���").ContinueWith(t => t.Result ? SaveAndClose() : Task.CompletedTask);
                return true;
            }
            return base.OnBackButtonPressed();
        }

        async Task SaveAndClose()
        {
            //await App.Current.SavePropertiesAsync();
            DependencyService.Get<IAndroidMethods>().CloseApp(); // AndroidMethods class provides this function
        }
    }

    public interface IAndroidMethods
    {
        void CloseApp();
    }
}