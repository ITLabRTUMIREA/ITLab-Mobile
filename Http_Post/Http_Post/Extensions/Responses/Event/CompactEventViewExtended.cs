using Models.PublicAPI.Responses.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Http_Post.Extensions.Responses.Event
{
    class CompactEventViewExtended : CompactEventView
    {
        public string Duration => TimeSpan.FromMinutes(TotalDurationInMinutes).ToString();
        public string EventTypeDescription => string.IsNullOrEmpty(EventType.Description) ? Res.Resource.NoDescriptionError : EventType.Description;
        public string EventTitle => string.IsNullOrEmpty(Title) ? Res.Resource.NoTitleError : Title;
        public string EventTypeTitle => string.IsNullOrEmpty(EventType.Title) ? Res.Resource.NoEventTypeTitleError : EventType.Title;
        public double ProgressToBar => Сompleteness / 100.0;
        public string СompletenessPercent => Convert.ToString(Сompleteness + " %");
    }
}
