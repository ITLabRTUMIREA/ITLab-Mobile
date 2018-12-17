using Models.PublicAPI.Requests.Events.Event.Edit;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Http_Post.Popup.Event
{
    class CreatePlace : TypeClass
    {
        public Task<PlaceEditRequest> AddPlaceEditRequest (INavigation navigation)
        {
            var tcs = new TaskCompletionSource<PlaceEditRequest>();
            entryDescription.Placeholder = "Введите описание места";
            entryTitle.Placeholder = "Введите кол-во людей";
            entryTitle.Keyboard = Keyboard.Numeric;
            btnOk.Clicked += async (s, e) =>
            {
                try
                {
                    var count = Convert.ToInt32(entryTitle.Text);
                    var description = entryDescription.Text;
                    await navigation.PopModalAsync();
                    tcs.SetResult(new PlaceEditRequest
                    {
                        TargetParticipantsCount = count,
                        Description = description,
                    });
                }
                catch (Exception)
                {
                    entryTitle.Focus();
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
