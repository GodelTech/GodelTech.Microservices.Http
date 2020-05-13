namespace GodelTech.Microservices.Http.Services
{
    public interface IJsonSerializer
    {
        T Deserialize<T>(string content);
        string Serialize(object data);
    }
}
