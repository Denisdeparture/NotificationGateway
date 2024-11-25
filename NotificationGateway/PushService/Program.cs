using BuisnesLogic.Extensions;
using BuisnesLogic.Models.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Data.Database;
using Data.interfaces;
using Data.Realization;
using BuisnesLogic.Models.Kafka;

namespace PushService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddKafkaClient<PushMessageTransportContract>(new KafkaConfig()
            {
                Host = builder.Configuration["KafkaConfig:Host"]!
            });
            builder.Services.AddDbContextFactory<DataBaseContext>(opt =>
            {
                opt.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]);
            });
            builder.Services.AddELKStack(new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                    optional: true)
                .Build());

            builder.Services.AddTransient<IMessageManager, MessageManager>();

            builder.Services.AddPushAll(builder.Configuration["PushAllConfig:Key"]!, builder.Configuration["PushAllConfig:Id"]!);

            builder.Services.AddHostedService<PushService<PushMessageTransportContract>>();

            var app = builder.Build();
            app.Run();
        }
    }
}
