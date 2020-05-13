using System;
using GodelTech.Microservices.Http.Services.ResponseHandlers;

namespace GodelTech.Microservices.Http.Services
{
    public interface IResponseHandlerFactory
    {
        IResponseHandler Create(Type type);
    }
}