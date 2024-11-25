using BuisnesLogic.Models.MessagesConfig;
using BuisnesLogic.Realization.Clients;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using FluentAssertions;
using Moq;

namespace xUnitTests.BuisnesLogicTests
{
    public class PushAllTests
    {
        private IPushAll client;
        [Fact]
        public void Test_When_Config_Was_Null()
        {
            // Act
            Action action = () => client = new PushAllClient(null);
            //Asserts
            Assert.Throws<ArgumentNullException>(() => action());
        }
        [Fact]
        public void Test_When_Params_Were_Null()
        {
            // Act
            Action action = () => client = new PushAllClient(new PushAllMessageModel(string.Empty, string.Empty));
            //Asserts
            Assert.Throws<ArgumentNullException>(() => action());
        }
        [Fact]
        public void Test_When_Params_In_Send_Were_Null()
        {
            //Arrange
            client = new PushAllClient(new PushAllMessageModel("test","test"));
            // Act
            Action action = () => client.Send(string.Empty, string.Empty);
            //Asserts
            Assert.Throws<ArgumentNullException>(() => action());
        }
    }
}
