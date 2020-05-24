using System;
using System.ServiceModel;

namespace MessageBus
{
    [ServiceContract]
    public interface ISubscriptionService
    {
        [OperationContract]
        void Subscribe(String pTopic, String pCallerAddress);

        [OperationContract]
        void Unsubscribe(String pTopic, String pCallerAddress);
    }
}
