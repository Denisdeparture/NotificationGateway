using BuisnesLogic.Models.MessagesConfig;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Realization.Clients
{
    public enum TypePush 
    {
        Self,
        Broadcast,
        Unicast,
        Multicast
    }
    public class PushAllClient : IPushAll, IDisposable
    {
        private static HttpClient _httpClient = null!;
        private readonly PushAllMessageModel _config;
        private const string Host = "https://pushall.ru/api.php";
        public PushAllClient(PushAllMessageModel config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            if (string.IsNullOrWhiteSpace(config.key)) throw new ArgumentNullException("key was null");
            if (string.IsNullOrWhiteSpace(config.id)) throw new ArgumentNullException("id was null");
            _httpClient = new HttpClient();
        }
        public void Send(string typePush, string text, string? url = null)
        {
            if(string.IsNullOrWhiteSpace(typePush)) throw new ArgumentNullException($"{nameof(typePush)} can`t be empty");
            if(string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException($"{nameof(text)} can`t be empty");
            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StringContent(_config.id.ToString()), name: "id");
                content.Add(new StringContent(typePush.ToString().ToLower()), name: "type");
                content.Add(new StringContent(_config.key), name: "key");
                content.Add(new StringContent(text), name: "title");
                if (url is not null) content.Add(new StringContent(url), name: "url");
                var task =  _httpClient.PostAsync(Host, content);
                task.Wait();
                var resp = task.Result;
                if (!resp.IsSuccessStatusCode) throw new Exception($"responce was {resp.StatusCode}");
            }
        }
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
