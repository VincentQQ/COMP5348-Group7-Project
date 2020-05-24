using System;

namespace MessageBus
{

    public class SubscriptionService : ISubscriptionService
    {
        public void Subscribe(string pTopic, string pCaller)
        {
            Console.WriteLine("Subscription for " + pTopic + " received");
            SubscriptionRegistry.Instance.AddSubscription(pTopic, pCaller);
        }

        public void Unsubscribe(string pTopic, string pCaller)
        {
            SubscriptionRegistry.Instance.RemoveSubscription(pTopic, pCaller);
        }
    }
}
