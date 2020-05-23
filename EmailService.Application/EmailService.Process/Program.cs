using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.ServiceLocatorAdapter;
using Microsoft.Practices.ServiceLocation;
using System.Configuration;
using System.Messaging;
using Common;
using EmailService.Services;
using System.Net;
using EmailService.Process.SubscriptionService;

namespace EmailService.Process
{
    class Program
    {
        private static global::Common.SubscriberServiceHost mhost;
        private const String eAddress = "net.msmq://localhost/private/EmailQueueTransacted";
        private const String eMexAddress = "net.tcp://localhost/private/EmailQueueTransacted/mex";
        static void Main(string[] args)
        {
            ResolveDependencies();
            //EnsureQueueExists();
            mhost = new SubscriberServiceHost(typeof(SubscriberService), eAddress, eMexAddress, true, ".\\private$\\EmailQueueTransacted");
            SubscribeForEvents();

            using (ServiceHost lHost = new ServiceHost(typeof(EmailService.Services.EmailService)))
            {
                lHost.Open();
                Console.WriteLine("Email Service Started");
                while (Console.ReadKey().Key != ConsoleKey.Q) ;
            }
        }

        private static void ResolveDependencies()
        {

            UnityContainer lContainer = new UnityContainer();
            UnityConfigurationSection lSection
                    = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            lSection.Containers["containerOne"].Configure(lContainer);
            UnityServiceLocator locator = new UnityServiceLocator(lContainer);
            ServiceLocator.SetLocatorProvider(() => locator);
        }
        private static void EnsureQueueExists()
        {
            // Create the transacted MSMQ queue if necessary.
            if (!MessageQueue.Exists(".\\private$\\EmailServiceQueue"))
                MessageQueue.Create(".\\private$\\EmailServiceQueue", true);
        }
        private static void SubscribeForEvents()
        {
            SubscriptionServiceClient eClient = new SubscriptionServiceClient();
            eClient.Subscribe("Email", eAddress);
        }
    }
}
