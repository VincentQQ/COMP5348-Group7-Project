using Common.Model;
using System.ServiceModel;

namespace MessageBus
{
    [ServiceContract]
    public interface IPublisherService
    {
        [OperationContract(IsOneWay = true)]
        void Publish(Message pMessage);
    }
}
