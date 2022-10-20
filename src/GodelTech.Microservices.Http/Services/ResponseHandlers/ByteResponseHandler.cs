using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GodelTech.Microservices.Http.Services.ResponseHandlers
{
    public class ByteResponseHandler : IResponseHandler
    {
        public async Task<T> ReadContentAsync<T>(HttpResponseMessage response)
        {
            if (typeof(T) != typeof(byte[]))
                throw new InvalidOperationException(nameof(ByteResponseHandler) + " supports only byte array result type");

            return (T) (object) await response.Content.ReadAsByteArrayAsync();
        }
    }
}
