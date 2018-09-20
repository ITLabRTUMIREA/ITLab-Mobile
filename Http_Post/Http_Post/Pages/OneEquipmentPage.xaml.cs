using System;
using Http_Post.Res;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Http_Post.Pages
{
	public partial class OneEquipmentPage : ContentPage
	{
		public OneEquipmentPage (Guid id)
		{
			InitializeComponent ();

            UpdateLanguage();
		}

        private void UpdateLanguage()
        {
            Title = Resource.Title_Equipment;
            Label_Type.Text = Resource.Equipment_Type;
            Label_Number.Text = Resource.Equipment_SerialNumber;
            Label_Description.Text = Resource.Equipment_Description;
            Label_OwnerTitle.Text = Resource.Equipment_Owner;
        }
    }
}