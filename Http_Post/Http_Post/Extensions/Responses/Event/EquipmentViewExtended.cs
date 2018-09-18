using Models.PublicAPI.Responses.Equipment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Http_Post.Extensions.Responses.Event
{
    class EquipmentViewExtended : EquipmentView
    {
        public string OwnerName { get; set; }
    }
}
