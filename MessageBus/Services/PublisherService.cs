using System;
using Common.Model;
using Common.Interfaces;

namespace MessageBus
{
    public class PublisherService : IPublisherService
    {
        public void Publish(Message pMessage)
        {
            foreach (String lHandlerAddress in SubscriptionRegistry.Instance.GetTopicSubscribers(pMessage.Topic))
            {
                ISubscriberService lSubServ = ServiceFactory.GetService<ISubscriberService>(lHandlerAddress);
                lSubServ.PublishToSubscriber(pMessage);
            }
        }
    }
}
