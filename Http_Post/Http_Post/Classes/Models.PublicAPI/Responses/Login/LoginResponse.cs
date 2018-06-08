﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses.Login
{
    public class LoginResponse
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentID { get; set; }
    }
}
