using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB._20532.Module08.AzureServiceBus.Topic
{
    class Program
    {
        static void Main(string[] args)
        {
            SendSubscribedMessage();
            ReceiveSubscribedMessage();
        }

        static void SendSubscribedMessage()
        {
            var conn = "Endpoint=sb://mohsin20532.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=KMFHDH/efxiyLexV1Rtym55iRJ5OPjg69gv4i/IMNUA=";

            var namespaceManager = NamespaceManager.CreateFromConnectionString(conn);

            if (!namespaceManager.TopicExists("UK"))
            {
                namespaceManager.CreateTopic("UK");
            }

            if (!namespaceManager.TopicExists("USA"))
            {
                namespaceManager.CreateTopic("USA");
            }


            var premiumFilter = new SqlFilter("Price > 2000");
            var economyFilter = new SqlFilter("Price <= 2000");


            if (!namespaceManager.SubscriptionExists("UK", "PremiumSubscription"))
            {
                namespaceManager.CreateSubscription("UK", "PremiumSubscription", premiumFilter);
            }
            if (!namespaceManager.SubscriptionExists("UK", "EconomySubscription"))
            {
                namespaceManager.CreateSubscription("UK", "EconomySubscription", economyFilter);
            }

            if (!namespaceManager.SubscriptionExists("USA", "PremiumSubscription"))
            {
                namespaceManager.CreateSubscription("USA", "PremiumSubscription", premiumFilter);
            }
            if (!namespaceManager.SubscriptionExists("USA", "EconomySubscription"))
            {
                namespaceManager.CreateSubscription("USA", "EconomySubscription", economyFilter);
            }

            var message = new BrokeredMessage("New Premium Booking");
            message.Properties["Price"] = 2500;

            var topicClient = TopicClient.CreateFromConnectionString(conn, "UK");

            topicClient.Send(message);

            Console.WriteLine("Sent to topic!");
            
        }

        static void ReceiveSubscribedMessage()
        {
            var conn = "Endpoint=sb://mohsin20532.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=KMFHDH/efxiyLexV1Rtym55iRJ5OPjg69gv4i/IMNUA=";
            var subscriptionClient = SubscriptionClient.CreateFromConnectionString(conn, "UK", "PremiumSubscription");

            OnMessageOptions options = new OnMessageOptions();
            options.AutoComplete = false;
            options.AutoRenewTimeout = TimeSpan.FromMinutes(1);

            subscriptionClient.OnMessage(message =>
            {
                try
                {
                    Console.WriteLine("Booking Description: " + message.GetBody<string>());
                    Console.WriteLine("Booking value: " + message.Properties["Price"].ToString());
                    message.Complete();
                }
                catch (Exception)
                {
                    message.Abandon();
                }

            },options);

            Console.Read();

        }
    }
}
