using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class StateMessageModel
    {
        public string? Content { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id {  get; set; }
        public string? Subject { get; set; }
        public DateTime TimeSended { get; set; }
        public string State { get; set; } = null!;
        public IList<BinaryDataModel>? Files { get; set; } 
        public string MessageType { get; set; } = null!;

    }
}
