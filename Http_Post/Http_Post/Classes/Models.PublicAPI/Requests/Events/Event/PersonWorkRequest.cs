using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Requests.Events.Event
{
    public class PersonWorkRequest : IdRequest
    {
        public DateTime? BeginWork { get; set; }
        public DateTime? EndWork { get; set; }
    }
}
