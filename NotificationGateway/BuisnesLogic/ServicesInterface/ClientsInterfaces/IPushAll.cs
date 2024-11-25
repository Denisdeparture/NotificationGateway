using BuisnesLogic.Realization.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.ServicesInterface.ClientsInterfaces
{
    public interface IPushAll
    {
        public void Send(string typePush, string text, string? url = null);
     
    }
}
