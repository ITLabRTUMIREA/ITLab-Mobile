using Models.PublicAPI.Responses.Event;
using System;
using System.Globalization;

namespace Http_Post.Extensions.Responses.Event
{
    public class CompactEventViewExtended : CompactEventView
    {
        // day of week
        public string DayOfWeek
            => BeginTime.ToString("dddd" , CultureInfo.GetCultureInfo(App.Current.Properties["language"].ToString()));
        // begin time
        public string BeginHour
            => BeginTime.ToLocalTime().Hour.ToString();
        // title
        public string EventTitle 
            => string.IsNullOrEmpty(Title) ? Res.Resource.ErrorNoTitle : Title;
        // duration
        public string Duration
            => (BeginTime.ToLocalTime() - EndTime.ToLocalTime()).ToString(@"dd\.hh\:mm");
        // type title
        public string EventTypeTitle 
            => string.IsNullOrEmpty(EventType.Title) ? Res.Resource.ErrorNoTitle : EventType.Title;
        // progress bar
        public double ProgressToBar
        => TargetParticipantsCount <= 0 ? 1 : Convert.ToDouble(CurrentParticipantsCount) / TargetParticipantsCount;
        // percents
        public string СompletenessPercent 
            => Convert.ToString((ProgressToBar*100).ToString("F0") + " %");
    }
}
