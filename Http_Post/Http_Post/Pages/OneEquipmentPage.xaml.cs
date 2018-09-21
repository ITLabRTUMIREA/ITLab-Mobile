using System;
using Http_Post.Res;
using Http_Post.Services;

using Xamarin.Forms;
using System.Net.Http;
using Http_Post.Extensions.Responses.Event;
using Newtonsoft.Json;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;

namespace Http_Post.Pages
{
	public partial class OneEquipmentPage : ContentPage
	{
        private Guid EquipId { get; }
        private HttpClient client = HttpClientFactory.HttpClient;
        private EquipmentViewExtended equipment { get; set; }

		public OneEquipmentPage (Guid id)
		{
			InitializeComponent ();

            EquipId = id;
            UpdateLanguage();
            GetEquip();
		}


        private async void GetEquip()
        {
            try
            {
                var response = await client.GetStringAsync($"Equipment/{EquipId}");
                var equip = JsonConvert.DeserializeObject<OneObjectResponse<EquipmentViewExtended>>(response);

                if (equip.Data.OwnerId != null)
                {
                    response = await client.GetStringAsync($"user/{equip.Data.OwnerId}");
                    var user = JsonConvert.DeserializeObject<OneObjectResponse<UserView>>(response);
                    equip.Data.OwnerName = user.Data.FirstName + " " + user.Data.LastName + ", " +
                        user.Data.Email;
                }
                else
                    equip.Data.OwnerName = Resource.ADMIN_Laboratory;

                equipment = equip.Data;
                SetInfo();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private void SetInfo()
        {
            Entry_Type.Text = equipment.EquipmentType.Title;
            Entry_Number.Text = equipment.SerialNumber;
            Entry_Description.Text = equipment.Description;
            Label_Owner.Text = equipment.OwnerName;
        }

        private void UpdateLanguage()
        {
            Title = Resource.Title_Equipment;
            Label_Type.Text = Resource.Equipment_Type;
            Label_Number.Text = Resource.Equipment_SerialNumber;
            Label_Description.Text = Resource.Equipment_Description;
            Label_OwnerTitle.Text = Resource.Equipment_Owner;
            /////////////////////////////////////////////////////
            Button_Confirm.Text = "Confirm";
            Button_ChangeOwner.Text = "Change Owner";
            Button_Delete.Text = "Delete";
            ////////////////////////////////////////////////////
            Button_Delete.BackgroundColor = Color.FromHex("#ff8080"); // Pretty red
        }

        private async void Button_Confirm_Clicked(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private async void Button_ChangeOwner_Clicked(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private async void Button_Delete_Clicked(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }
    }
}