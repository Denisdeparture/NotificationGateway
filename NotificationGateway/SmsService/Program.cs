using BuisnesLogic.Extensions;
using BuisnesLogic.Models.Kafka;
using BuisnesLogic.Models.Messages;
using BuisnesLogic.Realization;
using Data.Database;
using Data.interfaces;
using Data.Realization;
using Microsoft.EntityFrameworkCore;
using SmsAero;

namespace SmsService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddKafkaClient<SmsMessageTransportContract>(new KafkaConfig() { Host = builder.Configuration["KafkaConfig:Host"]! });

            builder.Services.AddDbContext<DataBaseContext>(opt =>
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

            builder.Services.AddTransient<SmsAeroClient>(opt => new SmsAeroClient(builder.Configuration["SmsAeroConfig:Email"]!, builder.Configuration["SmsAeroConfig:ApiKey"]!));

            builder.Services.AddHostedService<SmsService<SmsMessageTransportContract>>();

            var app = builder.Build();
            
            app.Run();
        }
    }
}
