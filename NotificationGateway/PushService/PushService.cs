using BuisnesLogic.Models.Messages;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using Data.interfaces;

namespace PushService
{
    public partial class PushService<T> : BackgroundService where T : PushMessageTransportContract
    {
        private readonly IMessageBrokerClient<T> _consumeClient;
        private readonly ILogger<PushService<T>> _logger;
        private readonly IPushAll _sender;
        private readonly IMessageManager _messageManager;
        private readonly IConfiguration _configuration;
        public PushService(IPushAll client, IMessageBrokerClient<T> consumeClient, ILogger<PushService<T>> logger, IMessageManager messageManager, IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageManager = messageManager ?? throw new ArgumentNullException(nameof(messageManager));
            _sender = client ?? throw new ArgumentNullException(nameof(client));
            _consumeClient = consumeClient ?? throw new ArgumentNullException(nameof(consumeClient));
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
                            _consumeClient.Consume(200, (model) => { ConsumerActions(model); }, cts, _configuration.GetValue<string>("KafkaConfig:DefaultTopics:PushChanel") ?? throw new NullReferenceException());
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
