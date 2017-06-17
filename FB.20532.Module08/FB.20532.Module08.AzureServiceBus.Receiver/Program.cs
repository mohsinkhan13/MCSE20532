using Microsoft.ServiceBus.Messaging;
using System;

namespace FB._20532.Module08.AzureServiceBus.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var conn = "Endpoint=sb://mohsin20532.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=KMFHDH/efxiyLexV1Rtym55iRJ5OPjg69gv4i/IMNUA=";
            var queueName = "orders";
            var queueClient = QueueClient.CreateFromConnectionString(conn, queueName);


            queueClient.OnMessage(message =>
            {
                Console.WriteLine("New Message : " + message.GetBody<string>());
            });

            Console.Read();
        }
    }
}
