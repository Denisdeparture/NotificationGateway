using BuisnesLogic.ConstStorage;
using BuisnesLogic.Extensions;
using BuisnesLogic.Models.Kafka;
using BuisnesLogic.Models.Messages;
using BuisnesLogic.Models.Requests;
using BuisnesLogic.Realization;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using Confluent.Kafka;
using BuisnesLogic.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Data.interfaces;
using BuisnesLogic.Models.Enums;
namespace NotificationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class HomeController : ControllerBase
    {
        private readonly IMetrics _metrics;
        private readonly MapServices _mapServices;
        private readonly ILogger<HomeController> _logger;
        private readonly IMessageBrokerClient<MessageTransportContract> _messageBroker;
        private readonly IMessageManager _messageManager;
        private readonly IConfiguration _configuration;
        public HomeController(IMetrics metrics, MapServices mapServices, ILogger<HomeController> logger, IMessageBrokerClient<MessageTransportContract> messageBroker, IMessageManager messageManager, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
            _messageBroker = messageBroker ?? throw new ArgumentNullException(nameof(messageBroker));
            _messageManager = messageManager ?? throw new ArgumentNullException(nameof(messageManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapServices = mapServices ?? throw new ArgumentNullException(nameof(mapServices));
        }
        [HttpPost]
        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public IActionResult Email([FromQuery] EmailRequest request)
        {
            if (request is null) return BadRequest($"{nameof(request)} was null");
            if (string.IsNullOrWhiteSpace(request.Message)) return BadRequest($"Message in request was null");
            var modelInDb = _messageManager.Add(_mapServices.MapFromDbModel(request, MessageType.Email));
            if (modelInDb is null) return BadRequest();
            ProduceInKafka<EmailRequest>(
                request, 
                modelInDb!, 
                _mapServices.MapFromKafkaModel<EmailRequest, MailMessageTransportContract>(request, (mess, model) =>
                {
                    try
                    {
                        model.config = AnalyzeMail(request.Mail!);
                    }
                    catch(Exception ex) 
                    {
                        _logger.LogError($"{DateTime.UtcNow} {ex.Message} {ex.StackTrace}");
                    }
                   
                }),
                _configuration.GetValue<string>("KafkaConfig:DefaultTopics:EmailChanel")!
                );
            _metrics.CounterRequestsEmail!.Add(1);
            return Ok();

        }
        [HttpPost]
        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public  IActionResult Sms([FromQuery] SmsRequest request)
        {
            if (request is null) return BadRequest($"{nameof(request)} was null");
            if (string.IsNullOrWhiteSpace(request.Message)) return BadRequest($"Message in request was null");
            var modelInDb = _messageManager.Add(_mapServices.MapFromDbModel(request, MessageType.Sms));
            if (modelInDb is null) return BadRequest();
            ProduceInKafka<Request>(
                request,
            modelInDb!,
                _mapServices.MapFromKafkaModel<Request, SmsMessageTransportContract>(request, (mess, model) =>
                {
                    try
                    {
                        model.SmsMessageConfig.NumberReceiver = request.NumberPhone;
                        model.Id = modelInDb.Id;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{DateTime.UtcNow} {ex.Message} {ex.StackTrace}");
                    }

                }),
                _configuration.GetValue<string>("KafkaConfig:DefaultTopics:SmsChanel")!
                );
            _metrics.CounterRequestsSms!.Add(1);
            return Ok();
        }
        [HttpPost]
        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public IActionResult Push([FromQuery] PushRequest request)
        {
            if (request is null) return BadRequest($"{nameof(request)} was null");
            if (string.IsNullOrWhiteSpace(request.Message)) return BadRequest($"Message in request was null");
            var modelInDb = _messageManager.Add(_mapServices.MapFromDbModel(request, MessageType.Push));
            if (modelInDb is null) return BadRequest();
            ProduceInKafka<Request>(
                request,
            modelInDb!,
                _mapServices.MapFromKafkaModel<Request, PushMessageTransportContract>(request, (mess, model) =>
                {
                    try
                    {
                        model.UriForWebPush = request.Uri;
                        model.TypePush = request.TypePush.ToString();
                        model.Id = modelInDb.Id;
                        model.Message = request.Message!;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{DateTime.UtcNow} {ex.Message} {ex.StackTrace}");
                    }

                }),
                _configuration.GetValue<string>("KafkaConfig:DefaultTopics:PushChanel")!
                );
            _metrics.CounterRequestsPush!.Add(1);
            return Ok();
        }
    }
}
