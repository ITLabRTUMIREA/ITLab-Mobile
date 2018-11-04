﻿using Http_Post.Classes;
using Http_Post.Pages;
using Xamarin.Forms;

namespace Http_Post
{
	public partial class Menu : TabbedPage
    {
        Localization localization = new Localization();

        public Menu()
        {
            InitializeComponent();

            AddPages(false);

            UpdateTheme();
        }

        void AddPages(bool isUpdating)
        {
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

        public void UpdateTheme()
        {
            var col = Application.Current.Resources;
            var th = new ThemeChanger().Theme;
            col["themeStack"] = col[th + "_Stack"];
            col["themeLabel"] = col[th + "_Lbl"];
            col["themeButton"] = col[th + "_Btn"];
            col["themeBar"] = col[th + "_Bar"];
        }

        public async void Logout()
            => await Navigation.PopToRootAsync();
    }
    
}