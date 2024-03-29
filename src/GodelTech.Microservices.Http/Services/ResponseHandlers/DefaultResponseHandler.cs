﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GodelTech.Microservices.Http.Services.ResponseHandlers
{
    public class DefaultResponseHandler : IResponseHandler
    {
        private readonly IJsonSerializer _jsonSerializer;

        public DefaultResponseHandler(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public Task<T> ReadContentAsync<T>(HttpResponseMessage response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            return ReadContentInternalAsync<T>(response);
        }

        private async Task<T> ReadContentInternalAsync<T>(HttpResponseMessage response)
        {
            var stringContent = await response.Content.ReadAsStringAsync();

            return string.IsNullOrWhiteSpace(stringContent) ?
                default :
                _jsonSerializer.Deserialize<T>(stringContent);
        }
    }
}
