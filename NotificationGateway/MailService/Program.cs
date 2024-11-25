using BuisnesLogic.Extensions;
using BuisnesLogic.Models.Messages;
using BuisnesLogic.Realization;
using Microsoft.EntityFrameworkCore;
using Data.Database;
using Data.interfaces;
using Data.Realization;
using BuisnesLogic.Models.Kafka;
namespace MailService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddKafkaClient<MailMessageTransportContract>(new KafkaConfig()
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

            builder.Services.AddSmtpClient(builder.Configuration);

            builder.Services.AddHostedService<MailService<MailMessageTransportContract>>();

            var app = builder.Build();
            //.WithName("GetWeatherForecast")
            //.WithOpenApi();
            app.Run();
        }
    }
}
