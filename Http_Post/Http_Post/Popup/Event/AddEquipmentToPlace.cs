using Models.PublicAPI.Requests;
using Models.PublicAPI.Responses.Equipment;
using Models.PublicAPI.Responses.General;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Http_Post.Popup.Event
{
    class AddEquipmentToPlace : TypeClass
    {
        public Task<DeletableRequest> AddEquipmentToPlaceAsync(INavigation navigation)
        {
            var tcs = new TaskCompletionSource<DeletableRequest>();
            layout.Children.Clear();
            StackLayout stack = new StackLayout();
            Label errorLabel = new Label();
            layout.Children.Add(lbl);
            layout.Children.Add(entryTitle);
            layout.Children.Add(stack);
            layout.Children.Add(slButtons);
            layout.Children.Add(errorLabel);
            entryTitle.Placeholder = "Введите оборудование";
            CompactEquipmentView tappedEquipment = null;
            entryTitle.TextChanged += async (s, e) =>
            {
                try
                {
                    stack.IsVisible = true;
                    var response = await Services.HttpClientFactory.HttpClient.GetStringAsync($"Equipment/?match={entryTitle.Text}");
                    var message = JsonConvert.DeserializeObject<ListResponse<CompactEquipmentView>>(response);

                    if (message.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                        throw new Exception($"Error: {message.StatusCode}");

                    foreach (var eq in message.Data)
                    {
                        Label lblType = new Label
                        {
                            Text = eq.EquipmentType.Title,
                            FontAttributes = FontAttributes.Bold
                        };
                        Label lblSerial = new Label
                        {
                            Text = eq.SerialNumber,
                            FontSize = lblType.FontSize - 4
                        };
                        StackLayout stackLayout = new StackLayout
                        {
                            Children = { lblType, lblSerial }
                        };
                        TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
                        tapGestureRecognizer.Tapped += (sen, args) =>
                        {
                            entryTitle.Text = eq.EquipmentType.Title;
                            stack.IsVisible = false;
                            tappedEquipment = eq;
                        };
                        stackLayout.GestureRecognizers.Add(tapGestureRecognizer);
                        stack.Children.Add(stackLayout);
                    }
                }
                catch (Exception ex)
                {
                    errorLabel.Text = ex.Message;
                }
            };
            btnOk.Clicked += async (s, e) =>
            {
                try
                {
                    tappedEquipment = tappedEquipment ?? throw new ArgumentNullException(nameof(tappedEquipment));

                    await navigation.PopModalAsync();
                    tcs.SetResult(new DeletableRequest
                    {
                        Id = tappedEquipment.Id,
                    });
                }
                catch (Exception ex)
                {
                    errorLabel.Text = ex.Message;
                }
            };
            btnCancel.Clicked += async (s, e) =>
            {
                await navigation.PopModalAsync();
                tcs.SetResult(null);
            };
            navigation.PushModalAsync(page);
            entryTitle.Focus();
            return tcs.Task;
        }
    }
}
