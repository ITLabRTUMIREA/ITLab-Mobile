using Models.PublicAPI.Requests.Equipment.EquipmentType;
using Models.PublicAPI.Requests.Events.Event.Edit.Roles;
using Models.PublicAPI.Requests.Events.EventType;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Http_Post.Popup.Types
{
    class EditTypePage : TypeClass
    {
        public EditTypePage()
        {

        }

        public Task<EventTypeEditRequest> eventTypeEdit(INavigation navigation, EventTypeEditRequest eventTypeEdit)
        {
            var tcs = new TaskCompletionSource<EventTypeEditRequest>();
            entryTitle.Text = eventTypeEdit.Title;
            entryDescription.Text = eventTypeEdit.Description;
            try
            {
                btnOk.Clicked += async (s, e) =>
                {
                    if (string.IsNullOrEmpty(entryTitle.Text) || string.IsNullOrWhiteSpace(entryTitle.Text))
                    {
                        entryTitle.Focus();
                        return;
                    }

                    var Type = new EventTypeEditRequest
                    {
                        Id = eventTypeEdit.Id,
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
                        Style = styleLbl,
                        HorizontalOptions = LayoutOptions.Center
                    });
                }
                return tcs.Task; // while Task != "good" -> invoke Task
            }
        }

        public Task<EventRoleEditRequest> eventRoleEdit(INavigation navigation, EventRoleEditRequest eventRoleEdit)
        {
            var tcs = new TaskCompletionSource<EventRoleEditRequest>();
            entryTitle.Text = eventRoleEdit.Title;
            entryDescription.Text = eventRoleEdit.Description;
            try
            {
                btnOk.Clicked += async (s, e) =>
                {
                    if (string.IsNullOrEmpty(entryTitle.Text) || string.IsNullOrWhiteSpace(entryTitle.Text))
                    {
                        entryTitle.Focus();
                        return;
                    }

                    var Type = new EventRoleEditRequest
                    {
                        Id = eventRoleEdit.Id,
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
                        Style = styleLbl,
                        HorizontalOptions = LayoutOptions.Center
                    });
                }
                return tcs.Task; // while Task != "good" -> invoke Task
            }
        }

        public Task<EquipmentTypeEditRequest> equipmentTypeEdit (INavigation navigation, EquipmentTypeEditRequest equipmentTypeEdit)
        {
            var tcs = new TaskCompletionSource<EquipmentTypeEditRequest>();
            entryTitle.Text = equipmentTypeEdit.Title;
            entryDescription.Text = equipmentTypeEdit.Description;

            Editor entryShortTitle = new Editor
            {
                Text = "",
                Placeholder = "Short title",
                Style = styleLbl
            };
            layout.Children.Clear();
            layout.Children.Add(lbl);
            layout.Children.Add(entryTitle);
            layout.Children.Add(entryShortTitle);
            layout.Children.Add(entryDescription);
            layout.Children.Add(slButtons);

            try
            {
                btnOk.Clicked += async (s, e) =>
                {
                    if (string.IsNullOrEmpty(entryTitle.Text) || string.IsNullOrWhiteSpace(entryTitle.Text))
                    {
                        entryTitle.Focus();
                        return;
                    }

                    var Type = new EquipmentTypeEditRequest
                    {
                        Id = equipmentTypeEdit.Id,
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
                        Style = styleLbl,
                        HorizontalOptions = LayoutOptions.Center
                    });
                }
                return tcs.Task; // while Task != "good" -> invoke Task
            }
        }
    }
}
