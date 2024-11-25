using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.ServicesInterface.ClientsInterfaces
{
    public interface IConsumeClient<TModel>
    {
        public void Consume(uint consuming_time, Action<TModel> action, CancellationTokenSource cancellationToken, string topic);
    }
}
