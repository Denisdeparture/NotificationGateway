using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using Castle.Core.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using BuisnesLogic.Realization;
namespace xUnitTests.BuisnesLogicTests
{
    public class MailServiceTests
    {
        private IConfiguration _configuration;
        private ISender _sender;
        [Fact]
        public void Test_When_Params_Were_Null()
        {
            //Arrange
            var mock = new Mock<IConfiguration>();
            _configuration = mock.Object;
            _sender = new MailSender(_configuration);
            //Act
            Action action = () => _sender.SendMailSmtp(null, null);
            //Assert
            Assert.Throws<ArgumentNullException>(action);
        }
    }
}
