using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Http_Post.Controls
{
    public partial class TypeView : ViewCell
    {
        public TypeView()
        {
            InitializeComponent();

            // TODO: check if field is null or empty
            if (string.IsNullOrEmpty(lbl.Text))
                lbl.IsVisible = false;
        }
    }
}
