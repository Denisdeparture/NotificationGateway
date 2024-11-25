using BuisnesLogic.Models;
using BuisnesLogic.Models.Kafka;
using BuisnesLogic.Models.MessagesConfig;
using BuisnesLogic.Service.Clients;
using BuisnesLogic.Realization;
using BuisnesLogic.Realization.Auth;
using BuisnesLogic.Realization.Clients;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;
using BuisnesLogic.Models.Messages;
using BuisnesLogic.Models.Other;
namespace BuisnesLogic.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddELKStack(this IServiceCollection services, IConfiguration configuration)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]!))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name!.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
                })
                .Enrich.WithProperty("Environment", environment)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            return services;
        }
        public static IServiceCollection AddJwtManager(this IServiceCollection services, JwtManagerConfig config) => services.AddTransient<IJwtManager, JwtManager>(x => new JwtManager(config));
        public static IServiceCollection AddKafkaClient<TModel>(this IServiceCollection services, KafkaConfig config) where TModel : MessageTransportContract
        {
            return services.AddTransient<IMessageBrokerClient<TModel>, KafkaClient<TModel>>(arg => new KafkaClient<TModel>(config));
        }
        public static IServiceCollection AddSmtpClient(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddTransient<ISender, MailSender>((obj) => new MailSender(configuration));
        }
        public static IServiceCollection AddMyMapService(this IServiceCollection services) => services.AddScoped<MapServices>();
        public static IServiceCollection AddPushAll(this IServiceCollection services, string key, string id) => services.AddTransient<IPushAll, PushAllClient>(x => new PushAllClient(new PushAllMessageModel(id, key)));

    }
}
