using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Models.Requests
{
    public class Request
    {
        public IFormFileCollection? FilesInAttachment { get; set; }
        public IFormFileCollection? FilesInBody { get; set; }
        [Required]
        public string Message { get; set; } = null!;
        public string? Subject { get; set; }
    }
}
