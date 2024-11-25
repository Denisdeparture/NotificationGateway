using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Models.Requests
{
    public class SmsRequest 
    {
        [Required]
        public string NumberPhone { get; set; } = null!;
        [Required]
        public string Message { get; set; } = null!;
        public static implicit operator Request(SmsRequest request)
        {
            return new Request()
            {
                Message = request.Message,
                Subject = "None",
                FilesInAttachment = null,
                FilesInBody = null,
            };
        }
    }
}
