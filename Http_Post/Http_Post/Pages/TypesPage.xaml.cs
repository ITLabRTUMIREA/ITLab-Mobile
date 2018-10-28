using Http_Post.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Http_Post.Pages
{
	public partial class TypesPage : ContentPage
	{
		public TypesPage ()
		{
			InitializeComponent ();
            UpdateLanguage();
		}

        private void UpdateLanguage()
        {
            Title = Resource.TitleTypes;
            lblEventTypes.Text = Resource.EventType;
            lblRoles.Text = "Roles";
            lblEquipmentTypes.Text = Resource.EquipmentType;
            btnCreate.Text = Resource.Create;
        }
	}
}