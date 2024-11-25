using BuisnesLogic.Models.Messages;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using Data.interfaces;
using MimeKit.Cryptography;

namespace ThirdPartyServices
{
    public partial class OtherService<T> : BackgroundService where T : OtherServicesTransportModel
    {
        private readonly ILogger<OtherService<T>> _logger;
        private readonly IServiceAuthOperation _operation;
        private readonly IMessageBrokerClient<T> _consumeClient;
        private readonly IMessageManager _messageManager;
        public OtherService(ILogger<OtherService<T>> logger, IServiceAuthOperation operation, IMessageBrokerClient<T> consumeClient, IMessageManager messageManager)
        {
            _operation = operation ?? throw new ArgumentNullException(nameof(operation));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _consumeClient = consumeClient ?? throw new ArgumentNullException(nameof(consumeClient));
            _messageManager = messageManager ?? throw new ArgumentNullException(nameof(messageManager));
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var cts = new CancellationTokenSource();
            var allService = _operation.GetAll();
            var factoryTask = new TaskFactory(stoppingToken);
            List<Task> tasks = new List<Task>();
            foreach (var task in allService)
            {
                tasks.Add(factoryTask.StartNew(() =>
                {
                    try
                    {
                        _consumeClient.Consume(200, model => ConsumeActions(model), cts, task.TopicName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{DateTime.UtcNow} {ex.Message} {ex.StackTrace} ");
                        cts.Cancel();
                    }

                }));
            }

            return Task.CompletedTask;
        }
       
    }
}
