using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class BinaryDataModel
    {
        public int Id { get; set; }
        public Guid FileId { get; set; } 
        public bool IsAttachment { get; set; }
        public string Data { get; set; } = null!;
        public string? Name { get; set; }
        public StateMessageModel Message { get; set; } 
    }
}
