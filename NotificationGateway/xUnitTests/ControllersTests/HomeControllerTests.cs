using BuisnesLogic.Models.Messages;
using BuisnesLogic.Models.Requests;
using BuisnesLogic.Realization;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using Data.interfaces;
using Data.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService;
using NotificationService.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace xUnitTests.ControllersTests
{
    public class HomeControllerTests
    {
        private HomeController Controller { get; set; }
        private Mock<IMessageBrokerClient<MessageTransportContract>> _mockMessageBrokerService;
        private Mock<IConfiguration> _mockConf;
        private Mock<IMetrics> _mockMetrics;
        private Mock<ILogger<HomeController>> _mockLogger;
        private Mock<IMessageManager> _mockMesageManager;
        public HomeControllerTests()
        {
            _mockMetrics = new Mock<IMetrics>();

            var mockLogger = new Mock<ILogger<HomeController>>();

            _mockMessageBrokerService = new Mock<IMessageBrokerClient<MessageTransportContract>>();

            _mockLogger = mockLogger;

            var mockMessageManager = new Mock<IMessageManager>();
            mockMessageManager.Setup(x => x.Add(It.IsAny<StateMessageModel>()));
            mockMessageManager.Setup(x => x.Update(It.IsAny<StateMessageModel>()));
            mockMessageManager.Setup(x => x.GetAll()).Returns(new List<StateMessageModel>());

            _mockMesageManager = mockMessageManager;

            _mockConf = new Mock<IConfiguration>();
        }
        [Fact]  
        private async void Test_When_Query_Null_Email()
        {
            //Arrange
            Controller = new HomeController(_mockMetrics.Object, new MapServices(), _mockLogger.Object, _mockMessageBrokerService!.Object, _mockMesageManager.Object, _mockConf.Object);
            //Act
            var request = Controller.Email(new EmailRequest()
            {
                Message = null,
                Subject = null,
                FilesInAttachment = null,
                FilesInBody = null,
                Mail = null,
            });
            //Asserts
            var res = request as BadRequestObjectResult;
            res.Should().NotBeNull();
        }
        [Fact]
        private void Test_When_Query_Null_Sms()
        {
            Controller = new HomeController(_mockMetrics.Object, new MapServices(), _mockLogger.Object, _mockMessageBrokerService!.Object, _mockMesageManager.Object, _mockConf.Object);
            //Act
            var request = Controller.Push(new PushRequest()
            {
                Message = null,
                TypePush =  BuisnesLogic.Realization.Clients.TypePush.Self,
                Uri = null,
            });
            //Asserts
            var res = request as BadRequestObjectResult;
            res.Should().NotBeNull();
        }
        [Fact]
        private void Test_When_Query_Null_Push()
        {
            Controller = new HomeController(_mockMetrics.Object, new MapServices(), _mockLogger.Object, _mockMessageBrokerService!.Object, _mockMesageManager.Object, _mockConf.Object);
            //Act
            var request = Controller.Sms(new SmsRequest
            {
                Message = null,
                NumberPhone = null
            });
            //Asserts
            var res = request as BadRequestObjectResult;
            res.Should().NotBeNull();
        }

    }
}
