using System.ServiceModel;
using Common.Model;

namespace Common.Interfaces
{
    [ServiceContract]
    public interface ISubscriberService
    {
        [OperationContract(IsOneWay = true)]
        void PublishToSubscriber(Message pMessage);
    }
}