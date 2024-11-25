using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Models.Messages
{
    public record SmtpMessageModel(string nameCompanyOrAdministration, string RecipientEmail, string SenderEmail, string provider, string domain_region, int port = 465, bool useSsl = true);

}
