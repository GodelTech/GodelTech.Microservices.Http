namespace GodelTech.Microservices.Http.Services
{
    public interface IServiceClientFactory
    {
        IServiceClient Create(string serviceName, bool returnDefaultOn404 = false);
    }
}
