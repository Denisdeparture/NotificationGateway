using BuisnesLogic.Models.Messages;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using BuisnesLogic.Models;
using MimeKit;
using System.Text.RegularExpressions;
using Data.interfaces;

namespace MailService
{
    public partial class MailService<T> : BackgroundService where T : MailMessageTransportContract
    {
        private readonly IMessageBrokerClient<T> _consumeClient;
        private readonly ILogger<MailService<T>> _logger;
        private readonly ISender _mailSender;
        private readonly IMessageManager _messageManager;
        private readonly IConfiguration _configuration;
        public MailService(IMessageBrokerClient<T> consumeClient, ILogger<MailService<T>> logger, ISender mailSender, IMessageManager messageManager, IConfiguration configuration)
        {
            _consumeClient = consumeClient ?? throw new ArgumentNullException(nameof(consumeClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailSender = mailSender ?? throw new ArgumentNullException(nameof(mailSender));
            _messageManager = messageManager ?? throw new ArgumentNullException(nameof(messageManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            var factoryTask = new TaskFactory(stoppingToken);
            var tasks = new[]
            {
                factoryTask.StartNew(() =>
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            _consumeClient.Consume(200, (model) => { ConsumerActions(model); }, cts, _configuration.GetValue<string>("KafkaConfig:DefaultTopics:EmailChanel") ?? throw new NullReferenceException());
                        }
                        catch(Exception ex)
                        {
                            _logger.LogError($"{DateTime.UtcNow} {ex.Message} {ex.StackTrace} ");
                        }
                    }

                })
            };
            await Task.WhenAll(tasks);
        }
      
        
    }
}
