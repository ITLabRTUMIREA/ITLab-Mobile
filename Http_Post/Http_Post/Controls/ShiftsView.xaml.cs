using Xamarin.Forms;

namespace Http_Post.Controls
{
	public partial class ShiftsView : ViewCell
	{
		public ShiftsView (StackLayout stack)
		{
			InitializeComponent ();

            stackLayout.Children.Add(stack);
        }
    }
}