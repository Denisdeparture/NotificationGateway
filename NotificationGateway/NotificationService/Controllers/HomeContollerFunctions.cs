using BuisnesLogic.ConstStorage;
using BuisnesLogic.Models.Messages;
using BuisnesLogic.Models.Enums;
using BuisnesLogic.Models;
using Microsoft.AspNetCore.Mvc;
using BuisnesLogic.Models.Requests;
using Data.Models;

namespace NotificationService.Controllers
{
    public partial class HomeController : ControllerBase
    {
        private void ProduceInKafka<T>(T request, StateMessageModel model, MessageTransportContract transportModel, string topic) where T : Request
        {
            try
            {
                var cts = new CancellationTokenSource();
                _messageBroker.Produce(transportModel, cts, topic);
                _logger.LogInformation($"Message from request type of {typeof(T)} sended");
            }
            catch (Exception ex)
            {
                if (model is not null)
                {
                    model.State = State.Error.ToString();
                    _messageManager.Update(model);
                }
                _logger.LogError($"{DateTime.UtcNow} {ex.Message} {ex.StackTrace}");
            }
        }
        private SmtpMessageModel AnalyzeMail(string Mail)
        {
            if (Mail.Contains(ValueStorage.MailProvideGoogle)) return SmtpConfigsStorage.GetConfigFrom(Mail).GMAIL;
            if (Mail.Contains(ValueStorage.MailProvideYandex)) return SmtpConfigsStorage.GetConfigFrom(Mail).YANDEX;
            if (ValueStorage.MailProvideMail.Where(x => Mail.Contains(x)) is not null) return SmtpConfigsStorage.GetConfigFrom(Mail).MAIL;
           
            throw new NotImplementedException($"Provider in {Mail} not realise");
        }
        private IResult ErrorResult(string log)
        {
            _logger.LogError(log);
            return Results.BadRequest();
        }
    }
}
