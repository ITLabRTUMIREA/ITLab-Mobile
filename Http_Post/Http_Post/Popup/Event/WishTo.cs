using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Http_Post.Popup.Event
{
    class WishTo : TypeClass
    {
        public async Task<Guid> GetRoleIdAsync(INavigation navigation)
        {
            var tcs = new TaskCompletionSource<Guid>();
            try
            {
                var response = await Services.HttpClientFactory.HttpClient.GetStringAsync("eventrole");
                var roles = JsonConvert.DeserializeObject <ListResponse<EventRoleView>>(response).Data;
                layout.Children.Clear();
                foreach (var role in roles)
                {
                    var btn = new Button {
                        Text = role.Title,
                        HorizontalOptions = LayoutOptions.Center,
                        WidthRequest = 150,
                    };
                    btn.Clicked += async (s, e) =>
                    {
                        await navigation.PopModalAsync();
                        tcs.SetResult(role.Id);
                    };
                    layout.Children.Add(btn);
                }
                btnCancel.HorizontalOptions = LayoutOptions.Center;
                btnCancel.WidthRequest = 150;
                btnCancel.Margin = new Thickness(0, 50);
                layout.Children.Add(btnCancel);
                btnCancel.Clicked += async (s, e) =>
                {
                    // close page
                    await navigation.PopModalAsync();
                    // pass empty result
                    tcs.SetResult(Guid.Empty);
                };
                await navigation.PushModalAsync(page);
                return await tcs.Task;
            }
            catch (Exception ex)
            {
                var itisLabel = layout.Children[layout.Children.Count - 1];
                if (itisLabel.GetType() == lbl.GetType())
                {
                    ((Label)layout.Children[layout.Children.Count - 1]).Text = ex.Message;
                }
                else
                {
                    layout.Children.Add(new Label
                    {
                        Text = ex.Message,
                        HorizontalOptions = LayoutOptions.Center
                    });
                }
                return await tcs.Task; // while Task != "good" -> invoke Task
            }
        }
    }
}
