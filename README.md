# GodeTech.Microservices.Http

`GodeTech.Microservices.Http` is an advanced REST API client. It's a wrapper around the standard `HttpClient` [System.Net.Http](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient). Service client handles conversion of CRL objects into HTTP requests. E.g. developer should no longer spend time on serialization aod deserialization of DTOs into JSON. More time can be spend on business logic rather than communication infrastructure. `GodeTech.Microservices.Http` library supports `GET`, `POST`, `DELETE`, `PUT` requests.

## Quick Start

### Demo REST API

Example is based on dummy `RemoteWeatherReport` REST API. This API provides information about current weather for specific town. Request results are converted into DTO `WeatherReportDto` by service client automatically.

RemoteWeatherReport API endpoint can be found at  [http://localhost:2020/v1/remote-weather-report?town=London](http://localhost:2020/v1/remote-weather-report?town=London)

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
    "BaseAddress": "http://localhost:2020",
    "ExcludeAccessToken" : true,
    "Timeout" : "0:0:30"
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

## Service Client Creation Options

`IServiceClientFactory` must be used to obtain instance of `IServiceClient`. Factory uses data from configuration section to initialize service client. The following parameters must be provided to `Create()` method of factory:

| Property | Description |
|---|---|
| `ServiceName` | Name of a service client. You need to add settings for the service to `appsettings.json` |
| `ReturnDefaultOn404` | If value is `false` and remote endpoint returns 404 HTTP status code exception is thrown by service client. In some cases 404 status code is expected value and `null` is assumed to be valid result of request. In order to return `null` instead of throwing exception `true` value must be specified. |

