using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using DeliveryCo.Services;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.ServiceLocatorAdapter;
using Microsoft.Practices.ServiceLocation;
using System.Configuration;

namespace DeliveryCo.Process
{
    class Program
    {
        static void Main(string[] args)
        {
            ResolveDependencies();
            //using (ServiceHost lHost = new ServiceHost(typeof(DeliveryService)))
            //{
            //    try
            //    {
            //        lHost.Open(); //lHost = HostAccountService(lAddress);
            //    }
            //    catch (AddressAlreadyInUseException)
            //    {
            //        var endpoint1 = lHost.Description.Endpoints.FirstOrDefault();
            //        var oldAddress1 = endpoint1.Address;
            //        endpoint1.Address = new EndpointAddress(new Uri("net.tcp://localhost:9090/DeliveryService")
            //            , oldAddress1.Identity, oldAddress1.Headers);
            //        var endpoint2 = lHost.Description.Endpoints[1];
            //        var oldAddress2 = endpoint2.Address;
            //        endpoint2.Address = new EndpointAddress(new Uri("net.tcp://localhost:9090/DeliveryService/mex")
            //            , oldAddress2.Identity, oldAddress2.Headers);
            //        lHost.Open();
            //    }

            //    Console.WriteLine("Delivery Service started. Press Q to quit");
            //    while (Console.ReadKey().Key != ConsoleKey.Q) ;
            //}
            
            try
            {
                ServiceHost lHost = new ServiceHost(typeof(DeliveryService));
                
                lHost.Open(); //lHost = HostAccountService(lAddress);
                Console.WriteLine("Host Address: {0}", lHost.Description.Endpoints[0].Address.Uri);
                Console.WriteLine("Host Address mex: {0}", lHost.Description.Endpoints[1].Address.Uri);
            }
            catch (AddressAlreadyInUseException)
            {
                // backup server
                ServiceHost lHost = new ServiceHost(typeof(DeliveryService));
                var endpoint1 = lHost.Description.Endpoints.FirstOrDefault();
                var oldAddress1 = endpoint1.Address;
                endpoint1.Address = new EndpointAddress(new Uri("net.tcp://localhost:9090/DeliveryService")
                    , oldAddress1.Identity, oldAddress1.Headers);
                var endpoint2 = lHost.Description.Endpoints[1];
                var oldAddress2 = endpoint2.Address;
                endpoint2.Address = new EndpointAddress(new Uri("net.tcp://localhost:9090/DeliveryService/mex")
                    , oldAddress2.Identity, oldAddress2.Headers);
                Console.WriteLine("Backup Host Address: {0}", lHost.Description.Endpoints[0].Address.Uri);
                Console.WriteLine("Backup Host Address mex: {0}", lHost.Description.Endpoints[1].Address.Uri);
                lHost.Open();
            }

            Console.WriteLine("Delivery Service started. Press Q to quit");
            while (Console.ReadKey().Key != ConsoleKey.Q) ;


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
    }
}
