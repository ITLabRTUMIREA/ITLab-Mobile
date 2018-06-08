﻿using Models.PublicAPI.Responses.Equipment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses.Event
{
    public class EventPresent
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Address { get; set; }
        public int NeededParticipantsCount { get; set; }
        public EventTypePresent EventType { get; set; }
        public List<Guid> EquipmentIds { get; set; }
        public List<Guid> ParticipantsIds { get; set; }
    }
}
