
using Data.Database;
using Data.interfaces;
using Data.Models;
using Microsoft.Extensions.DependencyInjection;
namespace Data.Realization
{
    public class MessageManager : IMessageManager
    {
        private readonly DataBaseContext _database;
        public MessageManager(IServiceScopeFactory serviceFactory)
        {
            var scope = serviceFactory.CreateScope();
            _database = scope.ServiceProvider.GetService<DataBaseContext>() ?? throw new NullReferenceException(nameof(_database));
        }
        public StateMessageModel? Add(StateMessageModel message)
        {
            if(message is null) throw new ArgumentNullException(nameof(message));
            _database.Messages.Add(message);
            _database.SaveChanges();
            var model = GetAll().Where(p => p.Content == message.Content & p.MessageType == message.MessageType & p.Subject == message.Subject).FirstOrDefault();
            return model;

        }
        public IEnumerable<StateMessageModel> GetAll() => _database.Messages;
        public void Delete(StateMessageModel message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));
            _database.Remove(message);
            _database.SaveChanges();
        }
        public void Delete(Guid id)
        {
            var message = _database.Messages.Where(mess => mess.Id == id).SingleOrDefault();
            if (message is null) throw new NullReferenceException($"User was null + {this}");
            _database.Messages.Remove(message);
            _database.SaveChanges();

        }
        public void DeleteAll() 
        {
            _database.Messages.RemoveRange(_database.Messages);
            _database.SaveChanges();
        }

        public StateMessageModel? Get(Guid id) => _database.Messages.Where(mess => mess.Id == id).SingleOrDefault();
        public void Update(StateMessageModel message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));
            _database.Update(message);
            _database.SaveChanges();
        }
    }
}
