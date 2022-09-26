using System.Net.Http;
using System.Threading.Tasks;

namespace GodelTech.Microservices.Http.Services.ResponseHandlers
{
    public interface IResponseHandler
    {
        Task<T> ReadContent<T>(HttpResponseMessage response);
    }
}
