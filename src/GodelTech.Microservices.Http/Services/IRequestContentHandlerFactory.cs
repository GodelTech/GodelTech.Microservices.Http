using System;
using GodelTech.Microservices.Http.Services.RequestHandlers;

namespace GodelTech.Microservices.Http.Services
{
    public interface IRequestContentHandlerFactory
    {
        IRequestContentHandler Create(Type type);
    }
}
