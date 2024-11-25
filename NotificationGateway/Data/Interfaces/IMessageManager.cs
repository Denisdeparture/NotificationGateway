using Data.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.interfaces
{
    public interface IMessageManager
    {
        public IEnumerable<StateMessageModel> GetAll();
        public StateMessageModel? Add(StateMessageModel user);
        public void Update(StateMessageModel user);
        public void Delete(Guid id);

    }
}
