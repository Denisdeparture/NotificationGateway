using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.ConstStorage
{
    public static class ValueStorage
    {
        public static string MessageDelivered = DateTime.UtcNow + "|" + DateTime.Now + "Message delivered";
        public static string MessageSended = DateTime.UtcNow + "|" + DateTime.Now + "Message sended";
        public static string MailProvideYandex = "@yandex.ru";
        public static string MailProvideGoogle = "@gmail.com";
        public static string[] MailProvideMail = new string[] {"@mail.ru", "@internet.ru", "@bk.ru", "@list.ru", "@inbox.ru"};
        public static string TestTopicMessage = "Test";
    }
}
