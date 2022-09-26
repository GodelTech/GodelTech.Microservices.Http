using System;
using System.Net.Http;
using GodelTech.Microservices.Http.Services.RequestHandlers;

namespace GodelTech.Microservices.Http.Services
{
    public class RequestContentHandlerFactory : IRequestContentHandlerFactory
    {
        private readonly IJsonSerializer _jsonSerializer;

        public RequestContentHandlerFactory(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public IRequestContentHandler Create(Type type)
        {
            if (type == typeof(HttpContent))
                return new HttpRequestContentHandler();

            return new DefaultRequestContentHandler(_jsonSerializer);
        }
    }
}
