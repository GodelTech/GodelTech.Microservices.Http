using System;
using System.Collections.Generic;
using System.Net.Http;
using GodelTech.Microservices.Core;
using GodelTech.Microservices.Http.Services;
using GodelTech.Microservices.Http.Services.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace GodelTech.Microservices.Http
{
    public class ServiceClientInitializer : MicroserviceInitializerBase
    {
        public int TransientErrorRetryCount { get; set; } = 3;

        public ServiceClientInitializer(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            var serviceConfigs = GetServices();

            services.AddHttpClient();

            services.AddSingleton<IBearerTokenStorage, BearerTokenStorage>();
            services.AddSingleton<IJsonSerializer, JsonSerializer>();
            services.AddSingleton<IServiceRegistry>(new ServiceRegistry(serviceConfigs));
            services.AddTransient<IServiceClientFactory, ServiceClientFactory>();
            services.AddTransient<IResponseHandlerFactory, ResponseHandlerFactory>();
            services.AddTransient<IRequestContentHandlerFactory, RequestContentHandlerFactory>();

            services.AddTransient<RequestResponseLoggingHandler>();
            services.AddTransient<CorrelationIdHandler>();

            foreach (var (serviceName, serviceEndpoint) in serviceConfigs)
            {
                AddServiceHttpClient(services, serviceName, serviceEndpoint);
            }
        }

        private void AddServiceHttpClient(IServiceCollection services, string serviceName, IServiceConfig serviceEndpoint)
        {
            var builder = services.AddHttpClient(serviceName.ToLowerInvariant(), client =>
            {
                ConfigureHttpClient(serviceName, serviceEndpoint, client);
            });

            ConfigureHttpMessageHandlers(serviceName, serviceEndpoint, builder);
        }

        protected virtual void ConfigureHttpClient(string serviceName, IServiceConfig serviceEndpoint, HttpClient client)
        {
            client.BaseAddress = new Uri(serviceEndpoint.BaseAddress);
            client.Timeout = serviceEndpoint.Timeout;
        }

        protected virtual void ConfigureHttpMessageHandlers(string serviceName, IServiceConfig serviceEndpoint, IHttpClientBuilder builder)
        {
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .RetryAsync(TransientErrorRetryCount);

            var noOp = Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();

            // IMPORTANT: This component should be only dependent on SingleInstance() components.
            // Transient instances may cause difficulties with tracing side effects raised by their
            // state or other transient deps (for example, per-request auth).
            // https://github.com/aspnet/HttpClientFactory/issues/198
            // https://github.com/aspnet/Docs/issues/9306

            builder
                .AddHttpMessageHandler<CorrelationIdHandler>()
                .AddHttpMessageHandler(services => new BearerAccessTokenHandler(services.GetRequiredService<IBearerTokenStorage>(), serviceEndpoint.ExcludeAccessToken))
                .AddPolicyHandler(request => request.Method == HttpMethod.Get || request.Method == HttpMethod.Head ? retryPolicy : noOp)
                .AddHttpMessageHandler<RequestResponseLoggingHandler>();
        }

        private IDictionary<string, IServiceConfig> GetServices()
        {
            var configSection = new ServiceConfigSection();

            Configuration.Bind(configSection);

            return configSection.GetServices();
        }
    }
}
