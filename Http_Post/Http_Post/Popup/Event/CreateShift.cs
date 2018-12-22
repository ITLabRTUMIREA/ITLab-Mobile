using Http_Post.Res;
using Models.PublicAPI.Requests.Events.Event.Edit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Http_Post.Popup.Event
{
    class CreateShift : TypeClass
    {
        public Task<ShiftEditRequest> AddShiftEditRequest (INavigation navigation)
        {
            var tcs = new TaskCompletionSource<ShiftEditRequest>();
            var grid = new Grid
            {
                Padding = new Thickness(0, 40, 0, 0),
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            }; // init here in a way to use it in 'catch' block
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            try
            {
                var lbl = new Label
                {
                    Text = Resource.Create,
                    HorizontalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold,
                };
                #region Adding labels which will show user, where is beginig and ending time/date
                var lblBegin = new Label
                {
                    Text = Res.Resource.Begining,
                    HorizontalOptions = LayoutOptions.Center,
                };
                var lblEnd = new Label
                {
                    Text = Res.Resource.Ending,
                    HorizontalOptions = LayoutOptions.Center,
                };
                #endregion
                var entryDescription = new Editor
                {
                    Text = "",
                    Placeholder = Resource.Description,
                };
                #region Adding time/date pickers and create special layouts for them
                var beginDate = new DatePicker
                {
                    MinimumDate = DateTime.Now.AddDays(-3)
                };
                var endDate = new DatePicker
                {
                    MinimumDate = DateTime.Now.AddDays(-3)
                };
                var beginTime = new TimePicker { Format = "HH:mm" };
                var endTime = new TimePicker();
                #endregion
                #region Adding places label hints and places editros
                var lblPlaces = new Label
                {
                    Text = "Places quantity",
                    HorizontalOptions = LayoutOptions.Center,
                };
                var lblPeople = new Label
                {
                    Text = "People quantity",
                    HorizontalOptions = LayoutOptions.Center,
                };
                var editPlaces = new Editor
                {
                    Text = "",
                    Placeholder = lblPlaces.Text,
                    Keyboard = Keyboard.Numeric,
                };
                var editPeople = new Editor
                {
                    Text = "",
                    Placeholder = lblPeople.Text,
                    Keyboard = Keyboard.Numeric,
                };
                #endregion
                btnOk.Clicked += async (s, e) =>
                {
                    try
                    {
                        if (endDate.Date < beginDate.Date)
                            throw new Exception($"Error: {Resource.ErrorNoDate}"); // Ending date can't be less than begining date!

                        var placesCreate = new List<PlaceEditRequest>();
                        for (int i = 0; i < Convert.ToInt32(editPlaces.Text); i++)
                            placesCreate.Add(new PlaceEditRequest
                            {
                                TargetParticipantsCount = Convert.ToInt32(editPeople.Text)
                            });

                        var newShift = new ShiftEditRequest
                        {
                            Description = entryDescription.Text,
                            BeginTime = beginDate.Date + beginTime.Time,
                            EndTime = endDate.Date + endTime.Time,
                            Places = placesCreate
                        };
                        await navigation.PopModalAsync();
                        tcs.SetResult(newShift);
                    }
                    catch (FormatException)
                    {
                        editPeople.Focus();
                    }
                    catch (Exception)
                    {
                        
                    }
                };
                btnCancel.Clicked += async (s, e) =>
                {
                    await navigation.PopModalAsync();
                    tcs.SetResult(null);
                };
                #region adding views to grid
                grid.Children.Add(lbl, 0, 0);
                Grid.SetColumnSpan(lbl, 2);
                grid.Children.Add(entryDescription, 0, 1);
                Grid.SetColumnSpan(entryDescription, 2);
                grid.Children.Add(lblBegin, 0, 2);
                grid.Children.Add(lblEnd, 1, 2);
                grid.Children.Add(beginDate, 0, 3);
                grid.Children.Add(endDate, 1, 3);
                grid.Children.Add(beginTime, 0, 4);
                grid.Children.Add(endTime, 1, 4);
                grid.Children.Add(lblPlaces, 0, 5);
                grid.Children.Add(lblPeople, 1, 5);
                grid.Children.Add(editPlaces, 0, 6);
                grid.Children.Add(editPeople, 1, 6);
                grid.Children.Add(btnOk, 0, 7);
                grid.Children.Add(btnCancel, 1, 7);
                #endregion
                var page = new ContentPage();
                page.Content = grid;
                navigation.PushModalAsync(page);
                entryDescription.Focus();
                return tcs.Task;
            }
            catch (Exception ex)
            {
                var itisLabel = grid.Children[grid.Children.Count - 1];
                if (itisLabel.Equals(new Label()))
                {
                    ((Label)grid.Children[grid.Children.Count - 1]).Text = ex.Message;
                }
                else
                {
                    grid.Children.Add(new Label
                    {
                        Text = ex.Message,
                        HorizontalOptions = LayoutOptions.Center
                    }, 0, 8);
                }
                return tcs.Task; // while Task != "good" -> invoke Task
            }
        }

    }
}
