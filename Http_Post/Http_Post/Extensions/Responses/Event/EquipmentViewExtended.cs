﻿using Models.PublicAPI.Responses.Equipment;
using Models.PublicAPI.Responses.General;
using System;

namespace Http_Post.Extensions.Responses.Event
{
    public class EquipmentViewExtended : EquipmentView
    {
        public string OwnerName { get; set; }

        public static implicit operator EquipmentViewExtended(OneObjectResponse<EquipmentViewExtended> v)
        {
            throw new NotImplementedException();
        }
    }
}
