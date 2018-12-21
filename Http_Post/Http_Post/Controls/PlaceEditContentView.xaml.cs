using Models.PublicAPI.Requests.Events.Event.Edit;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using System;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Requests.Events.Event.Create;

namespace Http_Post.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlaceEditContentView : ContentView
    {
        Image pathCollapse = new Image
        {
            Source = "ArrowRight.png"
        };
        Image pathExpand = new Image
        {
            Source = "ArrowDown.png"
        };
        PlaceEditRequest placeEdit;
        HttpClient client = Services.HttpClientFactory.HttpClient;
        IEnumerable<EventRoleView> roles;
        int numberOfPlace;
        string txtMessage;

        public PlaceEditContentView (PlaceEditRequest placeEditRequest, int numberOfShift, int numberOfPlace)
		{
            this.numberOfPlace = numberOfPlace;
            txtMessage = numberOfShift.ToString() + numberOfShift.ToString();
            placeEdit = placeEditRequest;
			InitializeComponent ();

            lblPlaceNumber.Text = $"#{numberOfPlace} | {Res.Resource.Participants}: {placeEdit.Invited.Count} {Res.Resource.Of} {placeEdit.TargetParticipantsCount}";
            editDescription.Placeholder = Res.Resource.Description;
            editDescription.Text = placeEdit.Description;
            image.Source = pathCollapse.Source;
            btnInvite.Text = Res.Resource.Invite;
            btnAddEquipment.Text = Res.Resource.Add + " " + Res.Resource.TitleEquipment;

            AddEquipment();

            GetEventRoles();
            AddPeople();
        }

        void PlaceNumber_Tapped(object sender, System.EventArgs e)
        {
            if (image.Source.Equals(pathCollapse.Source))
                image.Source = pathExpand.Source;
            else
                image.Source = pathCollapse.Source;

            stackToHide.IsVisible = !stackToHide.IsVisible;
        }

        async void GetEventRoles()
        {
            var response = await client.GetStringAsync("eventrole");
            var message = JsonConvert.DeserializeObject<ListResponse<EventRoleView>>(response);

            roles = message.Data;
        }

        async void AddPeople()
        {
            await System.Threading.Tasks.Task.Delay(450);
            stackPart.Children.Clear();
            foreach(var person in placeEdit.Invited)
            {
                var lblName = new Label
                {
                    FontAttributes = FontAttributes.Bold
                };
                var lblMail = new Label
                {
                    FontSize = lblName.FontSize - 4
                };
                try
                {
                    var response = await client.GetStringAsync($"user/{person.Id}");
                    var message = JsonConvert.DeserializeObject<OneObjectResponse<UserView>>(response);

                    var p = message.Data;
                    EventRoleView e = roles.Single(eventRole => eventRole.Id == person.EventRoleId);
                    lblName.Text = p.FirstName + " " + p.LastName + ", " + e.Title;
                    lblMail.Text = p.Email;
                    
                }
                catch (Exception)
                {
                    lblName.Text = Res.Resource.ErrorNoData + " " + Res.Resource.Name;
                    lblMail.Text = Res.Resource.ErrorNoData;
                }
                stackPart.Children.Add(lblName);
                stackPart.Children.Add(lblMail);
            }

            stackPart.IsVisible = stackPart.Children.Count == 0 ? false : true;
            lblPlaceNumber.Text = $"#{numberOfPlace} | {Res.Resource.Participants}: {placeEdit.Invited.Count} {Res.Resource.Of} {placeEdit.TargetParticipantsCount}";
        }

        async void AddEquipment()
        {
            stackEquip.Children.Clear();
            foreach (var equipment in placeEdit.Equipment)
            {
                var lblType = new Label
                {
                    FontAttributes = FontAttributes.Bold
                };
                var lblSerial = new Label
                {
                    FontSize = lblType.FontSize - 4
                };
                try
                {
                    var response = await client.GetStringAsync($"Equipment/{equipment.Id}");
                    var message = JsonConvert.DeserializeObject<OneObjectResponse<Models.PublicAPI.Responses.Equipment.EquipmentView>>(response);

                    Models.PublicAPI.Responses.Equipment.EquipmentView e = message.Data;
                    lblType.Text = e.EquipmentType.Title;
                    lblSerial.Text = e.SerialNumber;
                }
                catch (Exception)
                {
                    lblType.Text = Res.Resource.ErrorNoData + " " + Res.Resource.EquipmentType;
                    lblSerial.Text = Res.Resource.ErrorNoData + " "+ Res.Resource.SerialNumber;
                }
                stackEquip.Children.Add(lblType);
                stackEquip.Children.Add(lblSerial);
            }

            stackEquip.IsVisible = stackEquip.Children.Count == 0 ? false : true;
        }

        async void btnInvite_Clicked(object sender, System.EventArgs e)
        {
            MessagingCenter.Subscribe<PersonWorkRequest>(this,
                txtMessage,
                (person) =>
                {
                    placeEdit.Invited.Add(person);
                    AddPeople();
                });
            await Navigation.PushModalAsync(new Popup.Event.AddPersonToPlacePage(txtMessage));
        }

        async void btnAddEquipment_Clicked(object sender, System.EventArgs e)
        {
            DeletableRequest new_equipment = await new Popup.Event.AddEquipmentToPlace().AddEquipmentToPlaceAsync(Navigation);
            if (new_equipment == null)
                return;

            placeEdit.Equipment.Add(new_equipment);
            AddEquipment();
        }

        void DeletePlace_Tapped(object sender, EventArgs e)
        {
            placeEdit.Delete = true;
            this.IsVisible = false;
        }

        void editDescription_TextChanged(object sender, TextChangedEventArgs e)
            => placeEdit.Description = editDescription.Text;
    }
}