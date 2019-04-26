using System;

namespace Http_Post.Services
{
    interface IConfiguration
    {
        Lazy<string> BaseUrl { get; }
        Lazy<Guid> AppCenterId { get; }
    }
}