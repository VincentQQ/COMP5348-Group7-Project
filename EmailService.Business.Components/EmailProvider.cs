using EmailService.Business.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.Business.Components
{
    public class EmailProvider : IEmailProvider
    {
        public void SendEmail(EmailService.Business.Entities.EmailMessage pMessage)
        {
            Console.WriteLine(DateTime.Now.ToString() + "    Received request  to send email to " + pMessage.ToAddresses);
            Console.WriteLine("Sending email to " + pMessage.ToAddresses + ": " + pMessage.Message);
        }
    }
}
