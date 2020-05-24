using System;
using System.Messaging;
using System.ServiceModel;
using System.ServiceModel.Description;
using Common.Interfaces;

namespace Common
{
    public class SubscriberServiceHost : IDisposable
    {
        private ServiceHost mHost;

        public SubscriberServiceHost(Type pServiceHostType, String pAddress, String pMexAddress, bool pCreateQueueIfNotExist = false, String pQueueAddress = null)
        {
            if (pCreateQueueIfNotExist && (pQueueAddress != null))
            {
                CreateQueueIfNotExist(pQueueAddress);
            }
            mHost = new ServiceHost(pServiceHostType);
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            mHost.Description.Behaviors.Add(smb);
            mHost.AddServiceEndpoint(typeof(ISubscriberService),
                new NetMsmqBinding(NetMsmqSecurityMode.None) { Durable = true }, pAddress);
            mHost.AddServiceEndpoint(typeof(IMetadataExchange),
                MetadataExchangeBindings.CreateMexTcpBinding(),
                pMexAddress);
            mHost.Open();
        }

        private void CreateQueueIfNotExist(String pQueueAddress)
        {
            // Create the transacted MSMQ queue if necessary.
            if (!MessageQueue.Exists(pQueueAddress))
                MessageQueue.Create(pQueueAddress, true);
        }

        public void Dispose()
        {
            if (mHost != null)
            {
                mHost.Close();
            }
        }
    }
}