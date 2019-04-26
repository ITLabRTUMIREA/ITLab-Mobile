using System;
using System.Collections.Generic;

namespace Http_Post.Services
{
    static class CurrentUserIdFactory
    {
        private static Guid Id { set; get; }
        private static List<string> Roles { set; get; }

        public static void FirstSet(Guid id, List<string> roles)
        {
            Id = id;
            Roles = new List<string>();
            Roles = roles;
        }

        public static Guid UesrId
        {
            get
            {
                return Id;
            }
        }

        public static List<string> UserRoles
        {
            get
            {
                return Roles;
            }
        }
    }
}