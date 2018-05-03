using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace NorthOps.Portal.Services
{
    public partial class EmailServices
    {
        private string Password = "n0r+H0p$@$1@";
        private string Email = "careers@northops.asia";
        private string Host = "secure.serverpanels.com";
        private int Port = 587;
        private bool Ssl = true;
        public  Task SendEmailAsync(MailMessage mailMessage)
        {
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(host: Host, port: Port)
            {
                EnableSsl = Ssl,
                Credentials = new NetworkCredential(Email, Password),
            };
            mailMessage.From = new MailAddress(Email, "NorthOps");
          
          
            return  client.SendMailAsync(message: mailMessage);
        }
    }
}