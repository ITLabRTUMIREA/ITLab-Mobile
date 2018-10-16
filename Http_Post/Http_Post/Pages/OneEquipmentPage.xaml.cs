using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;
using Http_Post.Res;
using Http_Post.Services;
using Http_Post.Extensions.Responses.Event;

using System;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Http_Post.Pages
{
	public partial class OneEquipmentPage : ContentPage
	{
        private Guid EquipId { get; }
        private Guid OwnerId;
        private HttpClient client = HttpClientFactory.HttpClient;
        private EquipmentViewExtended equipment { get; set; }

        public OneEquipmentPage (Guid equipId)
		{
			InitializeComponent ();

            EquipId = equipId;
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
            lblType.Text = equipment.EquipmentType.Title;
            lblNumber.Text = equipment.SerialNumber;
            lblDescription.Text = equipment.Description;
            lblOwner.Text = equipment.OwnerName;
        }

        private void UpdateLanguage()
        {
            Title = Resource.Title_Equipment;
            lblTypeTitle.Text = Resource.Equipment_Type;
            lblNumberTitle.Text = Resource.Equipment_SerialNumber;
            lblDescriptionTitle.Text = Resource.Description;
            lblOwnerTitle.Text = Resource.Equipment_Owner;
            /////////////////////////////////////////////////////
            btnChange.Text = "Change";
        }

        private void btnChange_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CreateEquipment(equipment));
        }
    }
}