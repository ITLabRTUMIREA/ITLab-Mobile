using Xamarin.Forms;

namespace Http_Post.ToolBar
{
    class ToolBarItems
    {
        public ToolbarItem Item(string text, int priopity, ToolbarItemOrder itemOrder, string iconPath)
        {
            return new ToolbarItem
            {
                Text = text,
                Priority = 0,
                Order = itemOrder,
                Icon = iconPath
            };
        }
    }
}
