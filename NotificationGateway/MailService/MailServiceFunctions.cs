using BuisnesLogic.Models.Messages;
using BuisnesLogic.Models.Enums;
using MimeKit;
using System.Text.RegularExpressions;

namespace MailService
{
    public partial class MailService<T> : BackgroundService where T : MailMessageTransportContract
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
           
            _logger.LogWarning($"{model.config.domain_region} {model.config.provider} {model.config.SenderEmail} {model.config.RecipientEmail}");
            try
            {
                _mailSender.SendMailSmtp(ConfigMessage(model), model.config);
                _logger.LogInformation($"{DateTime.Now}: Message delivered {model.config.SenderEmail} ");
            }
            catch (Exception ex)
            {
                if (messageInDb is not null)
                {
                    messageInDb.State = State.Error.ToString();
                    _messageManager.Update(messageInDb);
                }
                _logger.LogError($" {DateTime.Now}: Message didn`t deliver, because {ex.StackTrace} {ex.Message}");
            }
        

            
        }
        private bool IsText(string text)
        {
            var regex = new Regex("<(?:\"[^\"]*\"['\"]*|'[^']*'['\"]*|[^'\">])+>");
            MatchCollection matches = regex.Matches(text);
            if (matches.Count > 0) return false;
            return true;
        }
        private MimeMessage ConfigMessage(T model)
        {
            var body = new BodyBuilder();
            if (IsText(model.Message ?? "") is true) body.TextBody = model.Message;
            else body.HtmlBody = model.Message;
            if (model.Files is not null)
            {
                foreach (var item in model.Files)
                {
                    if (item.Data is null)
                    {
                        _logger.LogWarning("data in file was null");
                        continue;
                    }
                    if (item.IsItAttachment) body.Attachments.Add(item.Name, item.Data);
                    else body.LinkedResources.Add(item.Name, item.Data);
                }
            }
            var message = new MimeMessage();
            message.Body = body.ToMessageBody();
            message.Subject = model.Subject;
            return message;
        }
    }
}
