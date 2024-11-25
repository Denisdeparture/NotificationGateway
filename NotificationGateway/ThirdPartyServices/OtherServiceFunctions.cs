using BuisnesLogic.Models.Messages;
using Data.Models.Enums;

namespace ThirdPartyServices
{
    public partial class OtherService<T> : BackgroundService where T : OtherServicesTransportModel
    {
        private void ConsumeActions(OtherServicesTransportModel? model)
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
            using (var httpClient = new HttpClient())
            {
                if (model.Headers is not null)
                {
                    foreach (var headers in model.Headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(headers.Key, headers.Value);
                    }
                }
                httpClient.PostAsync(model.OutputHttpRequest, null);
            }
        }
    }
}
