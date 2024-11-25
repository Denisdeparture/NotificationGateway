using BuisnesLogic.Realization.Clients;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Models.Requests
{
    public class PushRequest   
    {
        public string? Uri { get; set; }
        [Required]
        public TypePush TypePush { get; set; }
        [Required]
        public string Message { get; set; } = null!;
        public static implicit operator Request(PushRequest request)
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
