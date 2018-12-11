using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Http_Post.Popup.Equipment
{
    class Owner : TypeClass
    {
        public Task <UserView> Change(INavigation navigation)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<UserView>();
            var stack = new StackLayout
            {
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Vertical,
                IsVisible = false,
            };
            layout.Children.Clear();
            layout.Children.Add(lbl);
            layout.Children.Add(entryTitle);
            layout.Children.Add(stack);
            layout.Children.Add(slButtons);
            UserView newUser = new UserView();
            try
            {
                // while user types - find owners
                entryTitle.TextChanged += async (sender, e) =>
                {
                    try
                    {
                        stack.IsVisible = true;
                        stack.Children.Clear();
                        var txt = (Editor)sender;
                        var response = await client.GetStringAsync($"user?email={txt.Text}&count=10&firstname={txt.Text}&lastname={txt.Text}");
                        var users = JsonConvert.DeserializeObject<PageOfListResponse<UserView>>(response);
                        if (users.StatusCode != Models.PublicAPI.Responses.ResponseStatusCode.OK)
                            throw new Exception($"Error: {users.StatusCode}");

                        foreach (var u in users.Data)
                        {
                            var tapGestureRecognizer = new TapGestureRecognizer();
                            tapGestureRecognizer.Tapped += (s, args) =>
                            {
                                stack.IsVisible = false;
                                newUser = u;
                                entryTitle.Text = newUser.FirstName + " " + newUser.LastName;
                            };
                            var label = new Label
                            {
                                Text = u.FirstName + " " + u.LastName + ", " + u.Email
                            };
                            label.GestureRecognizers.Add(tapGestureRecognizer);
                            stack.Children.Add(label);
                            if (stack.Children.Count >= 5)
                                break;
                        };
                    }
                    catch (Exception ex)
                    {
                        stack.Children.Add(new Label
                        {
                            Text = ex.Message,
                        });
                    }
                };
                btnOk.Clicked += async (s, e) =>
                {
                    if (newUser == null)
                    {
                        entryTitle.Focus();
                        return;
                    }

                    await navigation.PopModalAsync();
                    // pass result
                    tcs.SetResult(newUser);
                };
                btnCancel.Clicked += async (s, e) =>
                {
                    // close page
                    await navigation.PopModalAsync();
                    // pass empty result
                    tcs.SetResult(null);
                };
                navigation.PushModalAsync(page);
                entryTitle.Focus();
                return tcs.Task;
            }
            catch (Exception ex)
            {
                var itisLabel = layout.Children[layout.Children.Count - 1];
                if (itisLabel.Equals(new Label()))
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
                return tcs.Task; // while Task != "good" -> invoke Task
            }
        }
    }
}
