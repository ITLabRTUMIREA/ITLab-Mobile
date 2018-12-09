using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EventView : ViewCell
	{
        public EventView ()
        {
            InitializeComponent();

            lblPercent.BackgroundColor = progBar.ProgressColor;
        }

        public static readonly BindableProperty CustomProperty = BindableProperty.Create(
            "Custom",
            typeof(string),
            typeof(EventView),
            null,
            BindingMode.TwoWay,
            propertyChanged: ImageSourceChanged);

        public string Custom
        {
            get { return GetValue(CustomProperty) as string; }
            set { SetValue(CustomProperty, value); }
        }

        private static void ImageSourceChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            //(bindableObject as EventView).Style.Source = Convert.ToString(newValue);
            var col = Application.Current.Resources;
            var th = System.Convert.ToString(newValue);
            var obj = bindableObject as EventView;
            obj.frame.Style = col[th + "_Frame"] as Style;
            obj.grid.Style = col[th + "_Stack"] as Style;
            int counter = 0;
            foreach (var item in obj.grid.Children)
            {
                if (counter == 5)
                {
                    counter++;
                    continue;
                }
                item.Style = col[th + "_Lbl"] as Style;
                counter++;
            }
            obj.progBar.Style = col[th + "_Stack"] as Style;
        }
    }
}