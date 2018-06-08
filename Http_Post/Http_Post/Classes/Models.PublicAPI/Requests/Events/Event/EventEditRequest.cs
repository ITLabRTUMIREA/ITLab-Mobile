using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Requests.Events.Event
{
    public class EventEditRequest : IdRequest
    {
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Address { get; set; }
        public Guid? EventTypeId { get; set; }
        public List<Guid> AddEquipment { get; set; }
        public List<Guid> RemoveEquipment { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public int NeededParticipantsCount { get; set; }
    }
}
