using BuisnesLogic.Models.Enums;
using BuisnesLogic.Models.Messages;
namespace SmsService
{
    public partial class SmsService<T> : BackgroundService where T : SmsMessageTransportContract
    {
        private void ConsumerActions(T? model)
        {
            if (model is null) throw new NullReferenceException(nameof(model));

            var messageInDb = _messageManager.GetAll().Where(x => x.Id == model.Id).SingleOrDefault();

            if (messageInDb is null) _logger.LogError($"{DateTime.Now} Message in Db was Null");
            else
            {
                // если будет ругаться напиши новый об и в него все свойства
                messageInDb.State = State.Dilivered.ToString();
                _messageManager.Update(messageInDb);
            }
            try
            {
                var task = _smsClient.SmsSend(model.Message!, model.SmsMessageConfig.NumberReceiver ?? throw new NullReferenceException($"Message in {this} was null"));

                task.Wait();
                _logger.LogWarning($"{model.SmsMessageConfig.NumberReceiver} ");

                _logger.LogInformation($"{DateTime.Now}: Message: {model.SmsMessageConfig.Message} delivered {model.SmsMessageConfig.NumberReceiver} ");
            }
            catch (Exception ex)
            {
                if (messageInDb is not null)
                {
                    messageInDb.State = State.Error.ToString();
                    _messageManager.Update(messageInDb);
                }
                _logger.LogError($"{model.Message!}||{model.SmsMessageConfig.NumberReceiver} {ex.Message}"); 
            }

            
        }
    }
}
