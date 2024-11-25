using BuisnesLogic.Extensions;
using BuisnesLogic.Models.Messages;
using BuisnesLogic;
using BuisnesLogic.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using Serilog;
using System.Runtime.CompilerServices;
using Data.Database;
using Data.Realization;
using Data.interfaces;
using BuisnesLogic.Models.Other;
namespace NotificationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<IMetrics, MyMetrics>();
            builder.Services.AddKafkaClient<MessageTransportContract>(new BuisnesLogic.Models.Kafka.KafkaConfig()
            {
                Host = builder.Configuration["KafkaConfig:Host"]!,
                Topics = (builder.Configuration.GetSection("KafkaConfig:DefaultTopics").GetChildren().Select(s => s.Value) ?? throw new NullReferenceException())!
            });
            builder.Services.AddDbContext<DataBaseContext>(opt =>
            {
                opt.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]);
            });
            builder.Services.AddOpenTelemetry()
                .WithMetrics(builder =>
                {
                    builder.AddMeter(MyMetrics.Name).AddPrometheusExporter();
                });
            builder.Host.UseSerilog();
            builder.Services.AddELKStack(new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                    optional: true)
                .Build());

            builder.Services.AddTransient<IMessageManager, MessageManager>();

            builder.Services.AddTransient<IServiceAuthOperation, AuthServiceManager>();

            builder.Services.AddMyMapService();

            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = AuthExtensions.GetSymmetricSecurityKey(builder.Configuration["JwtSettings:SecurityKey"]!),
                    ValidateIssuerSigningKey = true,
                };
            });
            builder.Services.AddAuthorization();
            builder.Services.AddJwtManager(new JwtManagerConfig()
            {
               Isssuer = builder.Configuration["JwtSettings:Issuer"]!,
               Audince = builder.Configuration["JwtSettings:Audience"]!,
               ExpirationTimeInMinutes = double.Parse( builder.Configuration["JwtSettings:ExpirationTimeInMinutes"]!),
               SecurityKey = builder.Configuration["JwtSettings:SecurityKey"]!
            });
            builder.Services.AddHostedService<ErrorMessageHandler>();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddControllers();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            
            app.UseOpenTelemetryPrometheusScrapingEndpoint("metrics");

            app.Map("/", () => Results.Redirect("/swagger/index.html"));

            app.Run();

        }
    }
}
