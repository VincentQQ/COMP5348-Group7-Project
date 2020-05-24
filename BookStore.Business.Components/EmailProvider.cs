using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Common.Model;
using BookStore.Business.Components.Interfaces;
using BookStore.Business.Components.PublisherService;
using BookStore.Business.Entities.Model;
using Bookstore.Business.Components.Transformations;

namespace BookStore.Business.Components
{
    public class EmailProvider : IEmailProvider
    {
        public void SendMessage(EmailMessage pMessage)
        {
            EmailMessageItem lItem = new EmailMessageItem()
            {
                Date = DateTime.Now,
                Message = pMessage.Message,
                ToAddresses = pMessage.ToAddress
            };
            EmailMessageItemToNewEmailMessage lVisitor = new EmailMessageItemToNewEmailMessage();
            lVisitor.Visit(lItem);

            PublisherServiceClient lClient = new PublisherServiceClient();
            lClient.Publish(lVisitor.Result);
            Console.WriteLine("A new email to " + pMessage.ToAddress + "has been published to Email Queue at " + DateTime.Now);
        }
    }
}
