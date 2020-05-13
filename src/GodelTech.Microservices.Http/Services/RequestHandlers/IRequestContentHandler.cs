using System.Net.Http;

namespace GodelTech.Microservices.Http.Services.RequestHandlers
{
    public interface IRequestContentHandler
    {
        HttpContent Create(object content);
    }
}
