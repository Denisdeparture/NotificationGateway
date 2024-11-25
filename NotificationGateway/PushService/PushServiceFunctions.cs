using BuisnesLogic.Models.Messages;
using BuisnesLogic.Models.Enums;

namespace PushService
{
    public partial class PushService<T> where T : PushMessageTransportContract
    {
        private void ConsumerActions(T? model)
        {
            if (model is null) throw new NullReferenceException(nameof(model));

            var messageInDb = _messageManager.GetAll().Where(x => x.Id == model.Id).SingleOrDefault();

            if (messageInDb is null) _logger.LogError($"{DateTime.Now} Message in Db was Null");
            else
            {
                messageInDb.State = State.Dilivered.ToString();
                _messageManager.Update(messageInDb);
            }
            _logger.LogWarning($"{model.Message} || {model.UriForWebPush} || {model.TypePush} {model.Id}");
            try
            {
                _sender.Send(model.TypePush, model.Message, model.UriForWebPush);
                _logger.LogInformation($"{DateTime.Now}: Message delivered {model.Id} ");
            }
            catch (Exception ex)
            {
                if(messageInDb is not null)
                {
                    messageInDb.State = State.Error.ToString();
                    _messageManager.Update(messageInDb);
                }
                _logger.LogError($" {DateTime.Now}: Message didn`t deliver, because {ex.StackTrace} {ex.Message}");
            }
            

            
        }
    }
}
