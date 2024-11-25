using BuisnesLogic.Models;
using BuisnesLogic.Models.Messages;
using BuisnesLogic.Realization.Clients;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using Microsoft.Extensions.Configuration;
using SmsAero;
using Data.interfaces;

namespace SmsService
{
    public partial class SmsService<T> : BackgroundService where T : SmsMessageTransportContract
    {
        private readonly IMessageBrokerClient<T> _consumeClient;
        private readonly ILogger<SmsService<T>> _logger;
        private readonly IMessageManager _messageManager;
        private readonly IConfiguration _configuration;
        private readonly SmsAeroClient _smsClient;
        public SmsService(IMessageBrokerClient<T> consumeClient, ILogger<SmsService<T>> logger,  IMessageManager messageManager, SmsAeroClient client, IConfiguration configuration)
        {
            _consumeClient = consumeClient ?? throw new ArgumentNullException(nameof(consumeClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageManager = messageManager ?? throw new ArgumentNullException(nameof(messageManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _smsClient = client ?? throw new ArgumentNullException(nameof(client));
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
                    while (!cts.IsCancellationRequested) {
                        try
                        {
                            _consumeClient.Consume(200, (model) => { ConsumerActions(model); }, cts, _configuration.GetValue<string>("KafkaConfig:DefaultTopics:SmsChanel") ?? throw new NullReferenceException());
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
