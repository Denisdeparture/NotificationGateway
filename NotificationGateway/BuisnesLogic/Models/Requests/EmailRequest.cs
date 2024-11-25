using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Models.Requests
{
    public class EmailRequest : Request
    {
        [Required]
        [MaxLength(150)]
        [MinLength(10)]
        public string? Mail { get; set; }
    }
}
