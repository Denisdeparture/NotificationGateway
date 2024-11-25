using BuisnesLogic.Extensions;
using BuisnesLogic.Models.Messages;
using BuisnesLogic.Services;
using Data.Database;
using Data.interfaces;
using Microsoft.EntityFrameworkCore;
namespace ThirdPartyServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
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
            builder.Services.AddKafkaClient<OtherServicesTransportModel>(builder.Configuration["KafkaConfig:Host"]!);

            builder.Services.AddTransient<IMessageManager, MessageManager>();

            builder.Services.AddTransient<IServiceAuthOperation, AuthServiceManager>();

            builder.Services.AddHostedService<OtherService<OtherServicesTransportModel>>();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.Run();
        }
    }
}
