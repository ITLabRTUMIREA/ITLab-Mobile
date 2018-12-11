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
    }
}