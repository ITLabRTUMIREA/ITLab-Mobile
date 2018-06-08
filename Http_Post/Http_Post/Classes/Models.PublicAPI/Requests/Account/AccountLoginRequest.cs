using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Requests.Account
{
    public class AccountLoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
