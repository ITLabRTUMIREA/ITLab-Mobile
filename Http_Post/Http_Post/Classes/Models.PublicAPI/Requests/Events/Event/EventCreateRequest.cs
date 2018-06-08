using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Requests.Events.Event
{
    public class EventCreateRequest
    {
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }   
        public string Address { get; set; }
        public List<PersonWorkRequest> Participants { get; set; }
        public List<Guid> Equipment { get; set; }
        public Guid EventTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int NeededParticipantsCount { get; set; }

    }
}
