using System;

namespace GodelTech.Microservices.Http.Services
{
    public interface IBearerTokenStorage
    {
        IDisposable SetAccessToken(string value);
        string GetAccessToken();
    }
}
