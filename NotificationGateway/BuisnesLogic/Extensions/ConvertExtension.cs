using BuisnesLogic.Models.Kafka;
using BuisnesLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;

namespace BuisnesLogic.Extensions
{
    public static class ConvertExtension
    {
        public static IList<BinaryDataModel>? ConvertToBinaryFilesForDb(this IEnumerable<KafkaFileTransport>? obj)
        {
            IList<BinaryDataModel>? res = null;
            if (obj is not null)
            {
                res = new List<BinaryDataModel>();
                foreach (var file in obj)
                {
                    res.Add(new BinaryDataModel()
                    {
                        Data = Convert.ToBase64String(file.Data),
                        IsAttachment = file.IsItAttachment,
                        Name = file.Name,
                    });
                }
            }
            return res;
        }
    }
}
