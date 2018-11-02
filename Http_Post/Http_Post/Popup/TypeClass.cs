using Http_Post.Res;
using System.Net.Http;
using Xamarin.Forms;

namespace Http_Post.Popup
{
    class TypeClass
    {
        public HttpClient client = Services.HttpClientFactory.HttpClient;

        public Style styleLbl = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Lbl"] as Style;
        public Style styleBtn = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Btn"] as Style;
        public Style styleStack = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Stack"] as Style;

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
            lbl.Style = styleLbl;
            entryTitle.Style = styleLbl;
            entryDescription.Style = styleLbl;
            btnOk.Style = styleBtn;
            btnCancel.Style = styleBtn;
            layout.Style = styleStack;

            slButtons.Style = styleStack;
            slButtons.Children.Add(btnOk);
            slButtons.Children.Add(btnCancel);

            layout.Children.Add(lbl);
            layout.Children.Add(entryTitle);
            layout.Children.Add(entryDescription);
            layout.Children.Add(slButtons);

            page.Style = styleStack;
            page.Content = layout;
        }
    }
}
