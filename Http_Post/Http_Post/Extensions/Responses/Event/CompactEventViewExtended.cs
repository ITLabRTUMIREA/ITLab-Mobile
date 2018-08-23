using Models.PublicAPI.Responses.Event;
using System;

namespace Http_Post.Extensions.Responses.Event
{
    class CompactEventViewExtended : CompactEventView
    {
        public string Duration => TimeSpan.FromMinutes(TotalDurationInMinutes).ToString();
        public string EventTypeDescription => string.IsNullOrEmpty(EventType.Description) ? Res.Resource.NoDescriptionError : EventType.Description;
        public string EventTitle => string.IsNullOrEmpty(Title) ? Res.Resource.NoTitleError : Title;
        public string EventTypeTitle => string.IsNullOrEmpty(EventType.Title) ? Res.Resource.NoEventTypeTitleError : EventType.Title;
        public double ProgressToBar => Convert.ToDouble(CurrentParticipantsCount) / TargetParticipantsCount;
        public string СompletenessPercent => Convert.ToString((ProgressToBar*100).ToString("F0") + " %");
    }
}
