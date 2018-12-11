using Models.PublicAPI.Requests.Equipment.EquipmentType;
using Models.PublicAPI.Requests.Events.Event.Create.Roles;
using Models.PublicAPI.Requests.Events.EventType;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Http_Post.Popup.Types
{
    class CreateTypePage : TypeClass
    {
        public Task <EventTypeCreateRequest> eventTypeCreate (INavigation navigation)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<EventTypeCreateRequest>();
            entryTitle.Placeholder = Res.Resource.EventType;
            try
            {
                btnOk.Clicked += async (s, e) =>
                {
                    if (string.IsNullOrEmpty(entryTitle.Text) || string.IsNullOrWhiteSpace(entryTitle.Text))
                    {
                        entryTitle.Focus();
                        return;
                    }

                    var Type = new EventTypeCreateRequest
                    {
                        Title = entryTitle.Text,
                        Description = entryDescription.Text,
                    };
                    
                    await navigation.PopModalAsync();
                    // pass result
                    tcs.SetResult(Type);
                };
                btnCancel.Clicked += async (s, e) =>
                {
                    // close page
                    await navigation.PopModalAsync();
                    // pass empty result
                    tcs.SetResult(null);
                };
                navigation.PushModalAsync(page);
                // open keyboard
                entryTitle.Focus();
                return tcs.Task;
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
                return tcs.Task; // while Task != "good" -> invoke Task
            }
        }

        public Task<EventRoleCreateRequest> eventRoleCreate (INavigation navigation)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<EventRoleCreateRequest>();
            try
            {
                btnOk.Clicked += async (s, e) =>
                {
                    if (string.IsNullOrEmpty(entryTitle.Text) || string.IsNullOrWhiteSpace(entryTitle.Text))
                    {
                        entryTitle.Focus();
                        return;
                    }

                    var Type = new EventRoleCreateRequest
                    {
                        Title = entryTitle.Text,
                        Description = entryDescription.Text,
                    };

                    await navigation.PopModalAsync();
                    // pass result
                    tcs.SetResult(Type);
                };
                btnCancel.Clicked += async (s, e) =>
                {
                    // close page
                    await navigation.PopModalAsync();
                    // pass empty result
                    tcs.SetResult(null);
                };
                navigation.PushModalAsync(page);
                // open keyboard
                entryTitle.Focus();
                return tcs.Task;
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
                return tcs.Task; // while Task != "good" -> invoke Task
            }
        }

        public Task<EquipmentTypeCreateRequest> equipmentTypeCreate (INavigation navigation)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<EquipmentTypeCreateRequest>();
            try
            {
                var entryShortTitle = new Editor {
                    Text = "",
                    Placeholder = Res.Resource.TitleShort,
                };

                entryTitle.Placeholder = Res.Resource.EquipmentType;
                layout.Children.Clear();
                layout.Children.Add(lbl);
                layout.Children.Add(entryTitle);
                layout.Children.Add(entryShortTitle);
                layout.Children.Add(entryDescription);
                layout.Children.Add(slButtons);

                btnOk.Clicked += async (s, e) =>
                {
                    if (string.IsNullOrEmpty(entryTitle.Text) || string.IsNullOrWhiteSpace(entryTitle.Text))
                    {
                        entryTitle.Focus();
                        return;
                    }

                    var Type = new EquipmentTypeCreateRequest
                    {
                        Title = entryTitle.Text,
                        ShortTitle = entryShortTitle.Text,
                        Description = entryDescription.Text,
                    };

                    await navigation.PopModalAsync();
                    // pass result
                    tcs.SetResult(Type);
                };
                btnCancel.Clicked += async (s, e) =>
                {
                    // close page
                    await navigation.PopModalAsync();
                    // pass empty result
                    tcs.SetResult(null);
                };
                navigation.PushModalAsync(page);
                // open keyboard
                entryTitle.Focus();
                return tcs.Task;
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
                return tcs.Task; // while Task != "good" -> invoke Task
            }
        }
    }
}
