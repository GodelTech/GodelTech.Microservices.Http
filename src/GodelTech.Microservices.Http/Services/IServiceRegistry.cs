namespace GodelTech.Microservices.Http.Services
{
    public interface IServiceRegistry
    {
        IServiceConfig GetConfig(string serviceName);
    }
}