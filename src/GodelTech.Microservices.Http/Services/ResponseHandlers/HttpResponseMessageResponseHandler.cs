using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GodelTech.Microservices.Http.Services.ResponseHandlers
{
    public class HttpResponseMessageResponseHandler : IResponseHandler
    {
        public Task<T> ReadContent<T>(HttpResponseMessage response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            return Task.FromResult((T) (object) response);
        }
    }
}
