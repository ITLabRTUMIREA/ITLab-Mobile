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
            
            Children.Add(new EventPage());
            
            Children.Add(new EquipmentPage());
            
            Children.Add(new TypesPage());
            
            Children.Add(new Settings(this));
            
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
                DisplayAlert("", "Вы уверены что хотите выйти?", "Да", "Нет").ContinueWith(t => t.Result ? SaveAndClose() : Task.CompletedTask);
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