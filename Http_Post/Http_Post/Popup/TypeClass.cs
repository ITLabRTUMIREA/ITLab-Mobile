using Http_Post.Res;
using System.Net.Http;
using Xamarin.Forms;

namespace Http_Post.Popup
{
    class TypeClass
    {
        public HttpClient client = Services.HttpClientFactory.HttpClient;

        public StackLayout layout = new StackLayout
        {
            Padding = new Thickness(0, 40, 0, 0),
            VerticalOptions = LayoutOptions.StartAndExpand,
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            Orientation = StackOrientation.Vertical,
        };
        public Label lbl = new Label
        {
            Text = Resource.Create,
            HorizontalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
        };
        public Editor entryTitle = new Editor
        {
            Text = "",
        };
        public Editor entryDescription = new Editor
        {
            Text = "",
            Placeholder = Resource.Description,
        };
        public Button btnOk = new Button
        {
            Text = "Ok",
            WidthRequest = 100,
        };
        public Button btnCancel = new Button
        {
            Text = Resource.ADMIN_Cancel,
            WidthRequest = 100,
        };
        public StackLayout slButtons = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Center,
        };
        public ContentPage page = new ContentPage();

        public TypeClass()
        {
            slButtons.Children.Add(btnOk);
            slButtons.Children.Add(btnCancel);

            layout.Children.Add(lbl);
            layout.Children.Add(entryTitle);
            layout.Children.Add(entryDescription);
            layout.Children.Add(slButtons);

            page.Content = layout;
        }
    }
}
