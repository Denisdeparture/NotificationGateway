using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Confluent.Kafka;
using Newtonsoft.Json;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using BuisnesLogic.ConstStorage;
using BuisnesLogic.Models.Kafka;
using Confluent.Kafka.Admin;
using BuisnesLogic.Models.Messages;
using System.Diagnostics;
namespace BuisnesLogic.Service.Clients
{

    public sealed class KafkaClient<TModel> : IMessageBrokerClient<TModel> where TModel : MessageTransportContract
    {
        private ProducerConfig? _producerConf;
        private ConsumerConfig? _consumeConf;

        public KafkaClient(KafkaConfig config)
        {
            KafkaConfig(config);
        }
        public async Task<ProduceResultModel> Produce(TModel model, CancellationTokenSource cts, string topic)
        {
            var resModel = new ProduceResultModel() { Success = false};

            if(model is null) throw new ArgumentNullException(nameof(model));

            if (cts is null) throw new ArgumentNullException(nameof(cts));

            var json = JsonConvert.SerializeObject(model);

            if(string.IsNullOrEmpty(json))
            {
                throw new NullReferenceException(nameof(json));
            }
            var message = new Message<string, string>()
            {
                Key = string.Join(string.Empty,Enumerable.Range(0,12).Select(x => ((char)(new Random().Next(97,122))).ToString())),
                Value = json
            };
            using (var producer = new ProducerBuilder<string, string>(_producerConf).SetKeySerializer(Serializers.Utf8).Build())
            {
                try
                {
                    resModel.Success = true;
                    await producer.ProduceAsync(topic, message, cts.Token);
                }
                catch
                {
                    return resModel;
                }
               
                return resModel;        
            }
        }
        public void Consume(uint consuming_time, Action<TModel> action, CancellationTokenSource cts, string topic)
        {

            using (var consumer = new ConsumerBuilder<string, string>(_consumeConf).Build())
            {
                consumer.Subscribe(topic);

                while (!cts.IsCancellationRequested)
                {
                    ConsumeResult<string, string>? res = null;
                    do
                    {
                        res = consumer.Consume(TimeSpan.FromSeconds(consuming_time));
                        Task.Delay(TimeSpan.FromSeconds(consuming_time));
                    } while (res is null);

                    if (res.Message.Value == ValueStorage.TestTopicMessage) continue;
                    TModel? data = JsonConvert.DeserializeObject<TModel>(res.Message.Value);

                    if (data is null) throw new NullReferenceException("Model was null");

                    action(data);

                }
            }
        }
        private async void KafkaConfig(KafkaConfig config)
        {
            if(config is null) throw new ArgumentNullException(nameof(config));
            if(string.IsNullOrWhiteSpace(config.Host)) throw new ArgumentNullException($"{nameof(config)} host is null");
            if (string.IsNullOrWhiteSpace(config.ClientId)) throw new ArgumentNullException($"{nameof(config)} ClientId is null");
            if (string.IsNullOrWhiteSpace(config.GroupId)) throw new ArgumentNullException($"{nameof(config)} GroupId is null");
            _producerConf = new ProducerConfig()
            {
                BootstrapServers = config.Host,
                Acks = Acks.All,
                Partitioner = Partitioner.ConsistentRandom
            };
            _consumeConf = new ConsumerConfig()
            {
                BootstrapServers = config.Host,
                ClientId = config.ClientId,
                GroupId = config.GroupId,
            };
            var message = new Message<string, string>()
            {
                Key = string.Join(string.Empty, Enumerable.Range(0, 12).Select(x => ((char)(new Random().Next(97, 122))).ToString())),
                Value = ValueStorage.TestTopicMessage
            };
            if(config.Topics is not null)
            {
                var cts = new CancellationTokenSource();
                using (var producer = new ProducerBuilder<string, string>(_producerConf).SetKeySerializer(Serializers.Utf8).Build())
                {
                    foreach (var topic in config.Topics)
                    {
                        await producer.ProduceAsync(topic, message, cts.Token);

                    }
                }
               

            }
        }

    }
}
