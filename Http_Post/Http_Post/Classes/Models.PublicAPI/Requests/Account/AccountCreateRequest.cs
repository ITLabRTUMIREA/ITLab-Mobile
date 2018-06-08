using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Requests.Account
{
    public class AccountCreateRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserType UserType { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string StudentID { get; set; }
    }
}
