using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Common.Model
{
    [DataContract]
    public class NewEmailMessage : Message
    {
        [DataMember]
        public String ToAddresses { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public String Message { get; set; }
        [DataMember]
        public String FromAddresses { get; set; }
        [DataMember]
        public String CCAddresses { get; set; }
        [DataMember]
        public String BCCAddresses { get; set; }
    }
}