using System;
using System.Net.Http;
using System.Text;

namespace GodelTech.Microservices.Http.Services.RequestHandlers
{
    public class DefaultRequestContentHandler : IRequestContentHandler
    {
        private readonly IJsonSerializer _jsonSerializer;

        public DefaultRequestContentHandler(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public HttpContent Create(object content)
        {
            if (content == null)
                return null;

            return new StringContent(
                _jsonSerializer.Serialize(content),
                Encoding.UTF8,
                "application/json");
        }
    }
}
