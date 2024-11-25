using BuisnesLogic.Models.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.ServicesInterface.ClientsInterfaces
{
    public interface IProduceClient<TModel>
    {
        public Task<ProduceResultModel> Produce(TModel model, CancellationTokenSource cts, string topic);
    }
}
