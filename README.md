# GodeTech.Microservices.Http

`GodeTech.Microservices.Http` is an advanced http client. It's a wrapper around the standard HttpClient [System.Net.Http](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient). The main idea of `GodeTech.Microservices.Http` is **to be able to receive data from any APIs in needed format: DTO, byte[], string, Stream**. `GodeTech.Microservices.Http` library allows to send GET, POST, DELETE, PUT requests.

## Quick Start

### Source API
For example, it is necessary to get data from a remote "RemoteWeatherReport" API. The API finds current weather for a town and returns it in special format `T`, where `T` is a serialized DTO class `WeatherReportDto`.

RemoteWeatherReport API endpoint: http://localhost:2020/v1/remote-weather-report?town=London

`RemoteWeatherReportController.cs`

```csharp
[ApiController]
[Route("v1/remote-weather-report")]
public class RemoteWeatherReportController : ControllerBase
{
    private readonly IWeatherReportService _weatherReportService;

    public RemoteWeatherReportController(IWeatherReportService weatherReportService)
    {
        _weatherReportService = weatherReportService;
    }

    [HttpGet]
    public async Task<IActionResult> GetWeatherReportsAsync([FromQuery] string town)
    {
        WeatherReportDto weatherReports = await _weatherReportService.GetWeatherReportsAsync(town);

        return Ok(weatherReports);
    }
}
```

**NOTE:** `_weatherReportService` is type of contarct `IWeatherReportService` returns weather information about the requested town.

`WeatherReportDto.cs`
```csharp
public class WeatherReportDto
{
    public int Id { get; set; }

    public string Town { get; set; }

    public int Temperature { get; set; }
}
```

### Target app
When we need to get RemoteWeatherReport API data from another application, it is easy to use `IServiceClient` and method `GetAsync<T>`, where `T` is a special format.

`WeatherForecastService.cs`

```csharp
public class WeatherForecastService : IWeatherForecastService
{

    private readonly IServiceClientFactory _serviceClientFactory;

    public WeatherForecastService(IServiceClientFactory serviceClientFactory)
    {
        _serviceClientFactory = serviceClientFactory;
    }

    public async Task<WeatherForecastDto> GetWeatherForecastsAsync(string town)
    {
        using IServiceClient client = _serviceClientFactory.Create("RemoteWeatherReport");

        return await client.GetAsync<WeatherForecastDto>($"/v1/remote-weather-report?town={town}");
    }
}
```

`WeatherForecastDto.cs`
```csharp
public class WeatherForecastDto
{
    public int Id { get; set; }

    public string Town { get; set; }

    public int Temperature { get; set; }
}
```

The `IServiceClientFactory` factory creates a service client. The service client is configured in the application settings. The following configuration description need to be added to `appsettings.json`:

```json
"ServiceEndpoints": {
  "RemoteWeatherReport": {
    "BaseAddress": "http://localhost:2020"
  }
}
```

In the same way, we can get data like `byte[]` or `string`.
```csharp
public async Task<string> GetStringAsync()
{
    using var client = _serviceClientFactory.Create("RemoteWeatherReport");

    return await client.GetAsync<string>("/v1/precipitations/str");
}

public async Task<byte[]> GetFlagAsync()
{
    using var client = _serviceClientFactory.Create("RemoteWeatherReport");

    return await client.GetAsync<byte[]>("/v1/precipitations/img");
}
```
## Configuration Options

Easiest way to configure factory `IServiceClientFactory` class. The following table contains list of available settings:

| Property | Description |
|---|---|
| `ServiceName` | Title of a service client. Your need to add settings for the service to `appsettings.json` |
| `ReturnDefaultOn404` | if value is `true` a service client returns object `T` without additional info. Default value is `false`. |

