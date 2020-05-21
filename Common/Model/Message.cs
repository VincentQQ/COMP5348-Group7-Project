using System;
using System.Runtime.Serialization;


namespace Common.Model
{
    [DataContract]
    public abstract class Message : IVisitable
    {
        [DataMember]
        public String Topic { get; set; }


        public void Accept(IVisitor pVisitor)
        {
            pVisitor.Visit(this);
        }
    }
}