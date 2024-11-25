using BuisnesLogic.Models;
using Data.Database;
using Data.interfaces;
using Data.Models;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Realization
{
    public class AuthServiceManager : IServiceAuthOperation
    {
        private readonly DataBaseContext _database;
        public AuthServiceManager(IServiceScopeFactory serviceFactory)
        {
            var scope = serviceFactory.CreateScope();
           _database = scope.ServiceProvider.GetService<DataBaseContext>() ?? throw new NullReferenceException(nameof(_database));
        }

        public void DeleteService(string Name, string Password, Predicate<string> predicate)
        {
            if(string.IsNullOrWhiteSpace(Password)) throw new ArgumentNullException(nameof(Password));
            if(string.IsNullOrWhiteSpace(Name)) throw new ArgumentNullException(nameof(Name));
            if(predicate is null) throw new ArgumentNullException(nameof(predicate));
            var service = GetModel(Name, Password, predicate);

            if (service is null) throw new Exception($"Service doesn`t exist");

            _database.Services.Remove(service);

            _database.SaveChanges();
        }

        public IEnumerable<ServiceAuthModel> GetAll() => _database.Services;


        public ServiceAuthModel? GetModel(string Name, string Password, Predicate<string> predicate) => _database.Services.Where(o => o.Login == Name & predicate(Password)).SingleOrDefault();
        
        public void Register(ServiceAuthModel model, Predicate<string> predicate)
        {
            if(model is null) throw new ArgumentNullException(nameof(model));
            var service = GetModel(model.Login, model.Password, predicate);

            if (service is not null) throw new Exception($"Service {service} just exist");

            _database.Services.Add(model);

            _database.SaveChanges();

        }

        public void UpdateService(string Name, string Password, ServiceAuthModel newModel, Predicate<string> predicate)
        {
            if (string.IsNullOrWhiteSpace(Password)) throw new ArgumentNullException(nameof(Password));
            if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentNullException(nameof(Name));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            var service = GetModel(Name, Password, predicate);
            if(service is null) throw new Exception($"Service doesn`t exist");

            newModel.Id = service.Id;

            _database.Services.Update(newModel);

            _database.SaveChanges();
        }
    }
}
