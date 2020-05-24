using EmailService.MessageTypes;
using Common;
using Common.Model;


namespace EmailService.Services.Transformations
{
    public class SendEmail : IVisitor
    {
        public EmailMessage Result { get; set; }
        public void Visit(IVisitable pVisitable)
        {
            if (pVisitable is NewEmailMessage)
            {
                NewEmailMessage lMsg = pVisitable as NewEmailMessage;
                Result = new EmailMessage()
                {
                    ToAddresses = lMsg.ToAddresses,
                    Date = lMsg.Date,
                    Message = lMsg.Message,
                };
            }
        }
    }
}