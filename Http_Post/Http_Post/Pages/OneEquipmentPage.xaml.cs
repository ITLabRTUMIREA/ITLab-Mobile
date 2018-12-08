using Models.PublicAPI.Responses.Equipment;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;
using Http_Post.Res;
using Http_Post.Services;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OneEquipmentPage : ContentPage
	{
        private Guid EquipId;
        private HttpClient client = HttpClientFactory.HttpClient;
        private EquipmentView equipment;

        public OneEquipmentPage (Guid equipId, bool canEdit)
		{
			InitializeComponent ();

            EquipId = equipId;
            UpdateLanguage();
            GetEquip();

            ChangeToolBar(canEdit);
		}

        async void GetEquip()
        {
            try
            {
                var response = await client.GetStringAsync($"Equipment/{EquipId}");
                var equip = JsonConvert.DeserializeObject<OneObjectResponse<EquipmentView>>(response);

                if (equip.Data.OwnerId != null)
                {
                    response = await client.GetStringAsync($"user/{equip.Data.OwnerId}");
                    var user = JsonConvert.DeserializeObject<OneObjectResponse<UserView>>(response);
                    lblOwner.Text = user.Data.FirstName + " " + user.Data.LastName + ", " +
                        user.Data.Email;
                }
                else
                    lblOwner.Text = Resource.ADMIN_Laboratory;

                equipment = equip.Data;
                SetInfo();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        void SetInfo()
        {
            lblType.Text = equipment.EquipmentType.Title;
            lblNumber.Text = equipment.SerialNumber;
            lblDescription.Text = equipment.Description;
        }

        void UpdateLanguage()
        {
            Title = Resource.TitleEquipment;
            lblTypeTitle.Text = Resource.EquipmentType;
            lblNumberTitle.Text = Resource.SerialNumber;
            lblDescriptionTitle.Text = Resource.Description;
            lblOwnerTitle.Text = Resource.Owner;
        }

        async void btnChange_Clicked(object sender, EventArgs e)
            => await Navigation.PushAsync(new CreateEquipment(equipment));

        void ChangeToolBar(bool can)
        {
            ToolbarItems.Clear();
            if (!can)
                return;

            var itemChange = new ToolBar.ToolBarItems().Item(null, 0, ToolbarItemOrder.Primary, "EditPencil.png");
            itemChange.Clicked += btnChange_Clicked;
            ToolbarItems.Add(itemChange);
        }
    }
}