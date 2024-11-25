using BuisnesLogic.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.ConstStorage
{
    public class SmtpConfigsStorage
    {
        private string email;
        public SmtpConfigsStorage(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(email);
            this.email = email ?? throw new ArgumentNullException(nameof(email));
        }
        public static SmtpConfigsStorage GetConfigFrom(string email) => new SmtpConfigsStorage(email);
        public SmtpMessageModel YANDEX => new SmtpMessageModel("BT", email, "ilimbaevAshitaDenisLD@yandex.ru", "yandex", "ru");
        public SmtpMessageModel MAIL => new SmtpMessageModel("BT", email, "ashitaden@mail.ru", "mail", "ru");
        public SmtpMessageModel GMAIL => new SmtpMessageModel("BT", email, "denis11you777602@gmail.com", "google", "com");
    }
}
