using BuisnesLogic.Models.Kafka;
using BuisnesLogic.Models.Messages;
using BuisnesLogic.Service.Clients;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using FluentAssertions;
using Moq;

namespace xUnitTests.BuisnesLogicTests
{
    public class KafkaTests
    {
        private IMessageBrokerClient<MessageTransportContract>? _kafkaclient;
        #region Ctor
        [Fact]  
        public void Test_When_Config_For_Ctor_Is_Null()
        {
           
            //Arrange
            var cts = new CancellationTokenSource();
            //Act
            Action action = () => _kafkaclient = new KafkaClient<MessageTransportContract>(null); 
            //Assert
            Assert.Throws<ArgumentNullException>(() => action());
        }
        [Fact]
        public void Test_When_Args_For_Ctor_Is_Null()
        {
            //Arrange
            var cts = new CancellationTokenSource();
            //Act
            Action action = () => {
                _kafkaclient = new KafkaClient<MessageTransportContract>(new KafkaConfig()
                {
                    Host = string.Empty,
                    ClientId = string.Empty,
                    GroupId = string.Join(string.Empty, Enumerable.Range(0, 10).Select(x => " "))
                });
            };
            //Assert
            Assert.Throws<ArgumentNullException>(() => action());
           
        }
        #endregion
        #region Producer
        [Fact]
        public async void Test_Produce_When_CTS_Was_Cancel()
        {
            var cts = new CancellationTokenSource();
            //Arrange
            _kafkaclient = new KafkaClient<MessageTransportContract>(new KafkaConfig()
            {
                Host = "localhost:9092",
            });
            //Act
            cts.Cancel();
            var res = await _kafkaclient.Produce(new MessageTransportContract()
            {
                Message = "testMessage",
                Id = new Guid()
            }, cts, "testtopic");
            res.Success.Should().Be(false);
        }
        #endregion
        #region Consumer
        [Fact]
        public void Test_Consume_When_CTS_Cancel()
        {
            //Arrange
            var cts = new CancellationTokenSource();
            const string realTopic = "defaultTopic";
            bool chechres = false;
            _kafkaclient = new KafkaClient<MessageTransportContract>(new KafkaConfig()
            {
                Host = "localhost:9092",
            });
            //Act
            cts.Cancel();
            _kafkaclient.Consume(50, (model) =>
            {
                // This action won`t work
                chechres = true;
            }, cts, realTopic);
            //Assert
            Assert.False(chechres);
        }
        #endregion

    }
}
