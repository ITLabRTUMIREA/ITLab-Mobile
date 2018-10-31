using Http_Post.Res;
using Models.PublicAPI.Requests.Equipment.EquipmentType;
using Models.PublicAPI.Requests.Events.Event.Create.Roles;
using Models.PublicAPI.Requests.Events.EventType;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Http_Post.Popup.Types
{
    class CreateTypePage
    {
        HttpClient client = Services.HttpClientFactory.HttpClient;

        Style styleLbl = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Lbl"] as Style;
        Style styleBtn = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Btn"] as Style;
        Style styleStack = Application.Current.Resources[new Classes.ThemeChanger().Theme + "_Stack"] as Style;

        StackLayout layout = new StackLayout
        {
            Padding = new Thickness(0, 40, 0, 0),
            VerticalOptions = LayoutOptions.StartAndExpand,
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            Orientation = StackOrientation.Vertical,
        };
        Label lbl = new Label
        {
            Text = Resource.Create,
            HorizontalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
        };
        Editor entryTitle = new Editor
        {
            Text = "",
            Placeholder = Resource.EventType,
        };
        Editor entryDescription = new Editor
        {
            Text = "",
            Placeholder = Resource.Description,
        };
        Button btnOk = new Button
        {
            Text = "Ok",
            WidthRequest = 100,
        };
        Button btnCancel = new Button
        {
            Text = Resource.ADMIN_Cancel,
            WidthRequest = 100,
        };
        StackLayout slButtons = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Center,
        };
        ContentPage page = new ContentPage();

        public CreateTypePage()
        {
            lbl.Style = styleLbl;
            entryTitle.Style = styleLbl;
            entryDescription.Style = styleLbl;
            btnOk.Style = styleBtn;
            btnCancel.Style = styleBtn;
            layout.Style = styleStack;

            slButtons.Style = styleStack;
            slButtons.Children.Add(btnOk);
            slButtons.Children.Add(btnCancel);

            layout.Children.Add(lbl);
            layout.Children.Add(entryTitle);
            layout.Children.Add(entryDescription);
            layout.Children.Add(slButtons);

            page.Style = styleStack;
            page.Content = layout;
        }

        public Task <EventTypeCreateRequest> eventTypeCreate (INavigation navigation)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<EventTypeCreateRequest>();
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
                        Style = styleLbl,
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
                        Style = styleLbl,
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
                    Placeholder = "Shoft title", // TODO: localize
                    Style = styleLbl
                };
                var entryParentId = new Editor
                {
                    Placeholder = "Я не работаю"
                };

                layout.Children.Clear();
                layout.Children.Add(lbl);
                layout.Children.Add(entryTitle);
                layout.Children.Add(entryShortTitle);
                layout.Children.Add(entryDescription);
                layout.Children.Add(entryParentId);
                layout.Children.Add(slButtons);
                page.Content = layout;

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
