using System;
using Xamarin.Forms;

namespace Http_Post.Controls
{
	public partial class EventView : ViewCell
	{
        public EventView ()
        {
            InitializeComponent();

            UpdateTheme();
        }

        private void UpdateTheme()
        {
            Classes.ThemeChanger theme = new Classes.ThemeChanger();
            lblTitle.TextColor = theme.ColorLbl();
            lblTypeTitle.TextColor = theme.ColorLbl();
            lblDescription.TextColor = theme.ColorLbl();
            lblBeginTime.TextColor = theme.ColorLbl();
            lblDuration.TextColor = theme.ColorLbl();
            lblShiftsCount.TextColor = theme.ColorLbl();
            ProgBar.BackgroundColor = theme.ColorBtn(); // ! ! !
            lblCompletness.TextColor = theme.ColorLbl();
        }
    }
}