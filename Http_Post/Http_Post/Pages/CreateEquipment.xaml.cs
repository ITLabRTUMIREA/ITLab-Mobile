using Http_Post.Res;
using Models.PublicAPI.Requests.Equipment.Equipment;
using Models.PublicAPI.Responses.Equipment;
using Models.PublicAPI.Responses.Exceptions;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateEquipment : ContentPage
	{
        private HttpClient client = Services.HttpClientFactory.HttpClient;
        private Guid? ownerId; // if empty - laboratory, if not empty - has owner
        private Guid typeId;
        private bool isCreating; // if creating - true, if changing - false
        private Guid eqId; // if creating - null, if changing - use already existed

        // pass to this ctor if needed to change equipment
        public CreateEquipment (EquipmentView equipment)
        {
            Init(false);
            eqId = equipment.Id;

            editEquipType.Text = equipment.EquipmentType.Title;
            Hide();
            editSerialNumber.Text = equipment.SerialNumber; // serial number
            editDescription.Text = equipment.Description; // description
            SetOwner(equipment.OwnerId); // set owner name
            typeId = equipment.EquipmentType.Id; // equipment type
        }

        public CreateEquipment ()
        {
            Init(true);
            SetOwner(null);
        }
        
        void Init(bool creating)
        {
            InitializeComponent();
            UpdateLanguage();
            isCreating = creating;
            if (isCreating)
                btnChangeOwner.IsVisible = false;
            else
                btnChangeOwner.IsVisible = GetRight();
        }

        async void editEquipType_TextChanged(object sender, TextChangedEventArgs e)
        {
            Show();
            stack.Children.Clear();
            try
            {
                var lbl = (Editor)sender;
                var response = await client.GetStringAsync($"EquipmentType?match={lbl.Text}&all=true");
                var equipType = JsonConvert.DeserializeObject<ListResponse<EquipmentTypeView>>(response);
                if (equipType.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {equipType.StatusCode}");

                foreach (var eq in equipType.Data)
                {
                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += (s, args) => {
                        editEquipType.Text = eq.Title;
                        typeId = eq.Id;
                        editSerialNumber.Focus();
                        Hide();
                    };
                    var label = new Label
                    { 
                        Text = eq.Title
                    };
                    label.GestureRecognizers.Add(tapGestureRecognizer);
                    stack.Children.Add(label);
                    if (stack.Children.Count >= 5)
                        break;
                }

            }
            catch (Exception ex)
            {
                stack.Children.Add(new Label
                {
                    Text = ex.Message,
                });
            }
        }

        async void btnConfirm_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (typeId == null)
                    throw new Exception($"Error: {Resource.EquipmentType} is null");

                if (string.IsNullOrEmpty(editSerialNumber.Text) || string.IsNullOrWhiteSpace(editSerialNumber.Text))
                    throw new Exception($"Error: {Resource.SerialNumber} is null");

                Guid? ownerGuid = ownerId == null ? ownerId : ownerId;
                EquipmentCreateRequest equipmentCreate;
                EquipmentEditRequest equipmentEdit;
                string jsonContent = "";
                if (isCreating)
                {
                    equipmentCreate = new EquipmentCreateRequest
                    {
                        EquipmentTypeId = typeId,
                        SerialNumber = editSerialNumber.Text,
                        Description = editDescription.Text,
                        
                        // sorry, children on site
                    };
                    jsonContent = JsonConvert.SerializeObject(equipmentCreate);
                }
                else
                {
                    equipmentEdit = new EquipmentEditRequest
                    {
                        Id = eqId,
                        EquipmentTypeId = typeId,
                        SerialNumber = editSerialNumber.Text,
                        Description = editDescription.Text
                        // sorry, parents on site
                    };
                    jsonContent = JsonConvert.SerializeObject(equipmentEdit);
                }
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var result = isCreating ?
                    await client.PostAsync("Equipment/", content) : await client.PutAsync("Equipment/", content);
                var resultContent = await result.Content.ReadAsStringAsync();
                var message = JsonConvert.DeserializeObject<OneObjectResponse<EquipmentView>>(resultContent);
                if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {message.StatusCode}");

                await DisplayAlert("", Resource.ADMIN_Updated, "Ok");

                await Navigation.PushAsync(new OneEquipmentPage(message.Data.Id, true));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        async void btnChangeOwner_Clicked(object sender, EventArgs e)
        {
            try
            {
                var user = await new Popup.Equipment.Owner().Change(Navigation);
                var eqEdit = new EquipmentEditRequest
                {
                    Id = eqId
                };
                var content = new StringContent(JsonConvert.SerializeObject(eqEdit), Encoding.UTF8, "application/json");
                var request = await client.PostAsync($"Equipment/user/{user.Id}", content);
                var requestContent = await request.Content.ReadAsStringAsync();
                var message = JsonConvert.DeserializeObject<OneObjectResponse<EquipmentView>>(requestContent);
                if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {message.StatusCode}");

                await DisplayAlert("", Resource.ADMIN_Updated, "Ok");

                if (user != null)
                    SetOwner(user.Id);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        async void SetOwner(Guid? id)
        {
            if (id == null) {
                lblOwner.Text = Resource.ADMIN_Laboratory;
                ownerId = null;
                return;
            }
            try
            {
                var response = await client.GetStringAsync($"user/{id}");
                var u = JsonConvert.DeserializeObject<OneObjectResponse<UserView>>(response);
                if (u.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {u.StatusCode}");

                ownerId = u.Data.Id;
                lblOwner.Text = u.Data.FirstName + " " + u.Data.LastName + ", " + u.Data.Email;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        void UpdateLanguage()
        {
            Title = Resource.TitleCreateEquipment;
            btnConfirm.Text = Resource.Save;
            btnChangeOwner.Text = Resource.ChangeOwner;
            btnDelete.Text = Resource.Delete;
            lblEquipType.Text = editEquipType.Placeholder = Resource.EquipmentType;
            lblSerialNumber.Text = editSerialNumber.Placeholder = Resource.SerialNumber;
            lblDescription.Text = editDescription.Placeholder = Resource.Description;
            lblOwnerTitle.Text = Resource.Owner;
        }

        void Show()
            => stack.IsVisible = true;

        void Hide()
            => stack.IsVisible = false;

        bool GetRight()
        {
            string whatToCheck = "CanEditEquipmentOwner";
            foreach (var item in Services.CurrentUserIdFactory.UserRoles)
                if (item.Equals(whatToCheck))
                    return true;
            return false;
        }

        async void btnDelete_Clicked(object sender, EventArgs e)
        {
            try
            {
                var equipmentDelete = new EquipmentEditRequest
                {
                    Id = eqId,
                    Delete = true,
                };
                var jsonContent = JsonConvert.SerializeObject(equipmentDelete);
                var request = new HttpRequestMessage(HttpMethod.Delete, "Equipment")
                {
                    Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
                };
                var result = await client.SendAsync(request);
                var resultContent = await result.Content.ReadAsStringAsync();

                var message = JsonConvert.DeserializeObject<ExceptionResponse>(resultContent);
                if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {message.StatusCode}");

                await DisplayAlert("", Resource.ADMIN_Updated, "Ok");
                for (var counter = 1; counter < 2; counter++)
                {
                    Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                }
                await Navigation.PopAsync(); // pop twice
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }
    }
}