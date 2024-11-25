using BuisnesLogic.Models.Messages;
using BuisnesLogic.Models;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Controllers;
using NotificationService;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Configuration;
using BuisnesLogic.Realization.Auth;
using BuisnesLogic.Realization;
using Data.interfaces;
using Data.Models;
using BuisnesLogic.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using BuisnesLogic.Models.Requests.Auth;
using BuisnesLogic.Models.Other;
using FluentAssertions;
namespace xUnitTests.ControllersTests
{
    public class AuthControllerTests
    {
        private AuthorizationController Controller { get; set; }
        private Mock<IMessageBrokerClient<MessageTransportContract>> _mockMessageBrokerService;
        private IJwtManager _JwtManager;
        private Mock<IMetrics> _mockMetrics;
        private Mock<ILogger<AuthorizationController>> _mockLogger;
        private Mock<IServiceAuthOperation> _mockAuthManager;
        public AuthControllerTests()
        {
            _mockMetrics = new Mock<IMetrics>();

            _mockLogger = new Mock<ILogger<AuthorizationController>>();

        
            _JwtManager = new JwtManager(new JwtManagerConfig()
            {
                Audince = "Token-JWT-Control",
                Isssuer = "http://localhost",
                SecurityKey = "https://localhost-JWT.05252125689478080880857",
                ExpirationTimeInMinutes = 60,
            });


            var mockMessageManager = new Mock<IServiceAuthOperation>();
            mockMessageManager.Setup(x => x.Register(It.IsAny<ServiceAuthModel>(), It.IsAny<Predicate<string>>()));
            mockMessageManager.Setup(x => x.GetAll()).Returns(new List<ServiceAuthModel>());
            _mockAuthManager = mockMessageManager;
        }
        [Fact]
        private void Test_When_Query_Null_Registration()
        {
            //Arrange
            Controller = new AuthorizationController(_mockAuthManager.Object,_mockMetrics.Object,_mockLogger.Object, _JwtManager, new MapServices() );
            //Act
            var request = Controller.Registration(new AuthRequest()
            {
                Login = string.Empty,
                Password = string.Empty,
            });

            //Asserts
            var res = request as BadRequestObjectResult;
            res.Should().NotBeNull();
        }
        [Fact]
        private void Test_When_Query_Null_Login()
        {
            Controller = new AuthorizationController(_mockAuthManager.Object, _mockMetrics.Object, _mockLogger.Object, _JwtManager, new MapServices());
            //Act
            var request = Controller.Login(new AuthRequest()
            {
                Login = string.Empty,
                Password = string.Empty,
            });
            //Asserts;
            var res = request as BadRequestObjectResult;
            res.Should().NotBeNull();
        }
    }
}
