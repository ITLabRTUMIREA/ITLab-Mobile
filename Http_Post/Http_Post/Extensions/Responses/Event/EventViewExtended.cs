using Models.PublicAPI.Responses.Event;
using Http_Post.Res;

namespace Http_Post.Extensions.Responses.Event
{
    public class EventViewExtended : EventView
    {
        // Event title
        public string EventTitle
            => string.IsNullOrEmpty(Title) ? Resource.ErrorNoTitle : Title;
        // Event type title
        public string EventTypeTitle
            => string.IsNullOrEmpty(EventType.Title) ? Resource.ErrorNoTitle : EventType.Title;
        // Event description
        public string EventDescription
            => string.IsNullOrEmpty(Description) ? Resource.ErrorNoDescription : Description;
        // Event address
        public string EventAddress
            => string.IsNullOrEmpty(Address) ? Resource.ErrorNoAddress : Address;
    }
}