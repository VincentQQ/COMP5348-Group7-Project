using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Routing;

namespace DeliveryCoRoutingService
{
    public class Router
    {

        // Host the service within this EXE console application.
        public static void Main()
        {
            // Create a ServiceHost for the CalculatorService type.
            using (ServiceHost serviceHost =
                new ServiceHost(typeof(RoutingService)))
            {

                // Open the ServiceHost to create listeners         
                Console.WriteLine("The Routing Service configured, opening....");
                serviceHost.Open();
                Console.WriteLine("The Routing Service is now running.");
                Console.WriteLine("Press <ENTER> to terminate router.");

                // The service can now be accessed.
                Console.ReadLine();
            }
        }

    }
}
