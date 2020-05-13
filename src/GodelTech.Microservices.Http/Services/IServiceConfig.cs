using System;

namespace GodelTech.Microservices.Http.Services
{
    public interface IServiceConfig
    {
        string BaseAddress { get; }
        TimeSpan Timeout { get; }
        bool ExcludeAccessToken { get; }
    }
}
