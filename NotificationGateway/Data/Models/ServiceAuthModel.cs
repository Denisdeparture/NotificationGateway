using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class ServiceAuthModel
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string SaltForPassword { get; set; } = null!;
        public int Id { get; set; }
    }
}
