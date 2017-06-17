using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB._20532.Module08.AzureServiceBus.Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            var conn = "Endpoint=sb://mohsin20532.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=KMFHDH/efxiyLexV1Rtym55iRJ5OPjg69gv4i/IMNUA=";
            var queueName = "orders";
            var queueClient = QueueClient.CreateFromConnectionString(conn,queueName);

            var message = new BrokeredMessage("New Order");
            queueClient.Send(message);

            Console.WriteLine("Message sent !");
            Console.Read();
        }
    }
}
