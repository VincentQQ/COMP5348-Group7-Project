using System;
using Common;

namespace BookStore.Business.Entities.Model
{
    public class EmailMessageItem : IVisitable
    {
        public string Topic => "Email";
        public string ToAddresses { get; set; }
        public string FromAddresses { get; set; }
        public string CCAddresses { get; set; }
        public string BCCAddresses { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public void Accept(IVisitor pVisitor)
        {
            pVisitor.Visit(this);
        }
    }
}
