using BuisnesLogic.Models;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.interfaces
{
    public interface IServiceAuthOperation
    {
        public void Register(ServiceAuthModel model, Predicate<string> predicate);
        public ServiceAuthModel? GetModel(string Name, string Password, Predicate<string> predicate);
        public void UpdateService(string Name, string Password, ServiceAuthModel newModel, Predicate<string> predicate);
        public void DeleteService(string Name, string Password, Predicate<string> predicate);
        public IEnumerable<ServiceAuthModel> GetAll();
    }
}
