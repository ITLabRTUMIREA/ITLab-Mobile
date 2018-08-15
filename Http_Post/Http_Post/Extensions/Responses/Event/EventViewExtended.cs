using Models.PublicAPI.Responses.Event;
using Http_Post.Res;

namespace Http_Post.Extensions.Responses.Event
{
    class EventViewExtended : EventView
    {
        public string EventTitle 
            => string.IsNullOrEmpty(Title) ? Resource.NoTitleError : Title;
        public string EventDescription
            => string.IsNullOrEmpty(Description) ? Resource.NoDescriptionError : Description;
        public string EventAddress
            => string.IsNullOrEmpty(Address) ? Resource.NoAddressError : Address;
        public string EventTypeTitle
            => string.IsNullOrEmpty(EventType.Title) ? Resource.NoEventTypeTitleError : EventType.Title;
        public string EventTypeDescription
            => string.IsNullOrEmpty(EventType.Description) ? Resource.NoEventTypeDescriptionError : EventType.Description;
    }
}
