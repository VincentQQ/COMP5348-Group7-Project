﻿using System;
using System.Runtime.Serialization;


namespace Common.Model
{
    [DataContract]
    [KnownType(typeof(NewEmailMessage))]
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