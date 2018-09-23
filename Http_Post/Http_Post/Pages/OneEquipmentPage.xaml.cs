using System;
using Http_Post.Res;
using Http_Post.Services;

using Xamarin.Forms;
using System.Net.Http;
using Http_Post.Extensions.Responses.Event;
using Newtonsoft.Json;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;
using Models.PublicAPI.Responses.Equipment;
using System.Text;
using Models.PublicAPI.Responses.Exceptions;

namespace Http_Post.Pages
{
	public partial class OneEquipmentPage : ContentPage
	{
        private Guid EquipId { get; }
        private Guid OwnerId;
        private HttpClient client = HttpClientFactory.HttpClient;
        private EquipmentViewExtended equipment { get; set; }
        private Action updateEquip;

		public OneEquipmentPage (Guid id, Action action)
		{
			InitializeComponent ();

            updateEquip = action;
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
            Button_Confirm.Text = Resource.Equipment_BtnConfirm;
            Button_ChangeOwner.Text = Resource.Equipment_BtnChangeOwner;
            Button_Delete.Text = Resource.Equipment_BtnDelete;
            ////////////////////////////////////////////////////
            Button_Delete.BackgroundColor = Color.FromHex("#ff8080"); // Pretty red
        }

        private async void Button_Confirm_Clicked(object sender, EventArgs e)
        {
            try
            {
                // TODO: equipment type list while text changing
                bool save = await DisplayAlert("", Resource.ADMIN_Sure, Resource.ADMIN_Yes, Resource.ADMIN_No);
                if (save) {
                    EquipmentView equipmentView = new EquipmentView
                    {
                        Id = equipment.Id, // BY DEFAULT
                        EquipmentTypeId = equipment.EquipmentTypeId, // BY DEFAULT
                        EquipmentType = new EquipmentTypeView
                        {
                            Title = Entry_Type.Text,
                            Description = equipment.EquipmentType.Description,
                            Id = equipment.EquipmentTypeId,
                            Childs = equipment.EquipmentType.Childs, // BY DEFAULT
                            Parent = equipment.EquipmentType.Parent // BY DEFAULT
                        },
                        Description = Entry_Description.Text,
                        SerialNumber = Entry_Number.Text,
                        OwnerId = equipment.OwnerId
                    };

                    var jsonContent = JsonConvert.SerializeObject(equipmentView);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var result = await client.PutAsync("Equipment/", content);
                    string resultContent = await result.Content.ReadAsStringAsync();
                    var newEquip = JsonConvert.DeserializeObject<OneObjectResponse<EquipmentView>>(resultContent);

                    if (newEquip.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                        throw new Exception($"Error: {newEquip.StatusCode}");
                    else
                        await DisplayAlert("Message", Resource.ADMIN_Updated, "Ok");
                }
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
                // TODO: display new name
                // change equipment.OwnerId
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
                bool delete = await DisplayAlert("", Resource.ADMIN_Sure, Resource.ADMIN_Yes, Resource.ADMIN_No);
                if (delete)
                {
                    var jsonConent = $"{{\"id\":\"{equipment.Id}\"}}";

                    var request = new HttpRequestMessage(HttpMethod.Delete, "Equipment") {
                        Content = new StringContent(jsonConent, Encoding.UTF8, "application/json")
                    };
                    var result = await client.SendAsync(request);

                    string resultContent = await result.Content.ReadAsStringAsync();
                    var message = JsonConvert.DeserializeObject<ExceptionResponse>(resultContent);
                    if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                        throw new Exception($"Error: {message.StatusCode}");

                    await DisplayAlert("", Resource.ADMIN_Updated, "Ok");
                    updateEquip.Invoke();
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // TODO: set new Owner
        }

        private async void Entry_Find_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //var response = await client.GetStringAsync($"user/?email={Entry_Find.Text}&firstname={Entry_Find.Text}&lastname={Entry_Find.Text}");
                // PageOfListResponse ?
            }
            catch (Exception ex)
            {

            }
        }
    }
}