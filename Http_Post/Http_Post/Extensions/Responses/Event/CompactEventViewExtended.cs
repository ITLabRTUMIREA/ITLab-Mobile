using Models.PublicAPI.Responses.Event;
using System;

namespace Http_Post.Extensions.Responses.Event
{
    public class CompactEventViewExtended : CompactEventView
    {
        public string Duration
            => TimeSpan.FromMinutes(TotalDurationInMinutes).ToString(@"dd\.hh\:mm");
        public string EventTypeDescription 
            => string.IsNullOrEmpty(EventType.Description) ? Res.Resource.ErrorNoDescription : EventType.Description;
        public string EventTitle 
            => string.IsNullOrEmpty(Title) ? Res.Resource.ErrorNoTitle : Title;
        public string EventTypeTitle 
            => string.IsNullOrEmpty(EventType.Title) ? Res.Resource.ErrorNoTitle : EventType.Title;
        public double ProgressToBar
            => Convert.ToDouble(CurrentParticipantsCount) / TargetParticipantsCount;
        public string СompletenessPercent 
            => Convert.ToString((ProgressToBar*100).ToString("F0") + " %");
    }
}
