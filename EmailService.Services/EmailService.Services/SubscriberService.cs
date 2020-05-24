using System;
using Common.Interfaces;
using Common.Model;
using EmailService.Business.Components.Interfaces;
using EmailService.Services.Transformations;

namespace EmailService.Services
{
    public class SubscriberService : ISubscriberService
    {
        public void PublishToSubscriber(Message pMessage)
        {
            EmailService eService = new EmailService();
            if (pMessage is NewEmailMessage)
            {
                NewEmailMessage lMessage = pMessage as NewEmailMessage;
                SendEmail lVisitor = new SendEmail();
                lMessage.Accept(lVisitor);
                eService.SendEmail(lVisitor.Result);
            }
        }
    }
}