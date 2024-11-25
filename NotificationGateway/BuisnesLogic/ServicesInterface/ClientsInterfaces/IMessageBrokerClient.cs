﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.ServicesInterface.ClientsInterfaces
{
    public interface IMessageBrokerClient<TModel> : IProduceClient<TModel>, IConsumeClient<TModel>
    {
    }
}