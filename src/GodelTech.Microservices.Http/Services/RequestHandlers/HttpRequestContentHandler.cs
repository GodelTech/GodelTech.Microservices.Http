using System.Net.Http;

namespace GodelTech.Microservices.Http.Services.RequestHandlers
{
    public class HttpRequestContentHandler : IRequestContentHandler
    {
        public HttpContent Create(object content)
        {
            return (HttpContent) content;
        }
    }
}