using Models.PublicAPI.Requests.Events.Event.Create;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Http_Post.Popup.Event
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddPersonToPlacePage : ContentPage
	{
        HttpClient client = Services.HttpClientFactory.HttpClient;
        Dictionary<string, Guid> nameToGuid;
        UserView user;
        int num;

		public AddPersonToPlacePage (int num)
		{
            this.num = num;
			InitializeComponent ();

            btnCancel.Text = "Отмена";
            btnOk.Text = "Ok";
            editName.Placeholder = "Введите имя/фамилию/mail человека";

            GetRoles();
		}

        async void GetRoles()
        {
            try
            {
                var response = await client.GetStringAsync("eventrole");
                var message = JsonConvert.DeserializeObject<ListResponse<EventRoleView>>(response);

                if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {message.StatusCode}");

                nameToGuid = new Dictionary<string, Guid>();
                foreach(var role in message.Data)
                {
                    nameToGuid.Add(role.Title, role.Id);
                    pickerRoles.Items.Add(role.Title);
                }
            }
            catch (Exception ex)
            {
                errorLabel.Text = ex.Message;
            }

        }

        async void editName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                stackNames.Children.Clear();
                stackNames.IsVisible = true;
                var response = await client.GetStringAsync($"user?email={editName.Text}&count=10&firstname={editName.Text}&lastname={editName.Text}");
                PageOfListResponse<UserView> users = JsonConvert.DeserializeObject<PageOfListResponse<UserView>>(response);

                if (users.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                    throw new Exception($"Error: {users.StatusCode}");

                foreach (var u in users.Data)
                    stackNames.Children.Add(getStack(u));
            }
            catch (Exception ex)
            {
                errorLabel.Text = ex.Message;
            }
        }

        StackLayout getStack(UserView u)
        {
            Label lblName = new Label
            {
                Text = u.FirstName + " " + u.LastName,
                FontAttributes = FontAttributes.Bold
            };
            Label lblMail = new Label
            {
                Text = u.Email,
                FontSize = lblName.FontSize - 4
            };
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                editName.Text = u.FirstName + " " + u.LastName;
                user = u;
                stackNames.IsVisible = false;
            };
            StackLayout stack = new StackLayout
            {
                Children = { lblName, lblMail }
            };
            stack.GestureRecognizers.Add(tap);
            return stack;
        }

        async void btnOk_Clicked(object sender, EventArgs e)
        {
            if (user == null || editName.Text == String.Empty || pickerRoles.SelectedItem == null)
                return;

            MessagingCenter.Send<PersonWorkRequest>(new PersonWorkRequest
            {
                Id = user.Id,
                EventRoleId = nameToGuid[pickerRoles.SelectedItem.ToString()]
            }, num.ToString());
            await Navigation.PopModalAsync();
        }

        async void btnCancel_Clicked(object sender, EventArgs e)
            => await Navigation.PopModalAsync();
    }
}