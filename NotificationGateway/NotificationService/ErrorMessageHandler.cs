using BuisnesLogic.Models.Messages;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;

using BuisnesLogic.Models.Enums;
using Data.interfaces;

namespace NotificationService
{
    public class ErrorMessageHandler : BackgroundService
    {
        private readonly ILogger<ErrorMessageHandler> _logger;
        private readonly IMessageManager _messageManager;
        private readonly IConfiguration _configuration;
        private readonly IMessageBrokerClient<MessageTransportContract> _messageBroker;
        public ErrorMessageHandler(ILogger<ErrorMessageHandler> logger, IMessageManager messageManager, IConfiguration configuration, IMessageBrokerClient<MessageTransportContract> producer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageBroker = producer ?? throw new ArgumentNullException(nameof(producer));
            _messageManager = messageManager ?? throw new ArgumentNullException( nameof(messageManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            await Task.Run(() =>
            {
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        Task.Delay(600);
                        var messages = _messageManager.GetAll().Where(obj => obj.State == State.Error.ToString());
                        foreach (var message in messages)
                        {
                            if (message.MessageType == MessageType.Email.ToString())
                            {
                                _messageBroker.Produce(new MailMessageTransportContract(), cts, _configuration.GetValue<string>("KafkaConfig:DefaultTopics:EmailChanel") ?? throw new NullReferenceException());
                                message.State = State.Sended.ToString();
                                _messageManager.Update(message);
                            }
                            if (message.MessageType == MessageType.Push.ToString()) 
                            {
                                _messageBroker.Produce(new MailMessageTransportContract(), cts, _configuration.GetValue<string>("KafkaConfig:DefaultTopics:PushChanel") ?? throw new NullReferenceException());
                                message.State = State.Sended.ToString();
                                _messageManager.Update(message);
                            }
                            if (message.MessageType == MessageType.Sms.ToString()) 
                            {
                                _messageBroker.Produce(new MailMessageTransportContract(), cts, _configuration.GetValue<string>("KafkaConfig:DefaultTopics:SmsChanel") ?? throw new NullReferenceException());
                                message.State = State.Sended.ToString();
                                _messageManager.Update(message);
                            }
                        }


                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError($"{ex.Message} {ex.StackTrace}");
                }
               
            });
        }
    }
}
