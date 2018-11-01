using System;

namespace Http_Post.Services
{
    class CurrentUserIdFactory
    {
        private static Guid Id { set; get; }

        public void FirstSet(Guid id)
        {
            if (Id != Guid.Empty)
                return;
            Id = id;
        }

        public static Guid UesrId
        {
            get
            {
                return Id;
            }
        }
    }
}