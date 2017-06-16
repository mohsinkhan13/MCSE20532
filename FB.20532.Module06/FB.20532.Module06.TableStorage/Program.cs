using System;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FB._20532.Module06.TableStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            ReadBlob();
            ReadPrivateBlob();
            Console.ReadLine();
        }

        static void CreateOrder()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference("order");
            table.CreateIfNotExists();

            var order1 = new OrderEntity("Truck", "Truck1") { Description = "Diesel", sku = "T1" };
            var order2 = new OrderEntity("Truck", "Truck2") { Description = "Diesel", sku = "T2" };
            var order3 = new OrderEntity("Truck", "Truck3") { Description = "Diesel", sku = "T3" };

            //var tableOperation = TableOperation.Insert(order);
            var tableBatchOperation = new TableBatchOperation();
            tableBatchOperation.Insert(order1);
            tableBatchOperation.Insert(order2);
            tableBatchOperation.Insert(order3);
            table.ExecuteBatch(tableBatchOperation);


            Console.WriteLine("Order created !");
            Console.ReadLine();
        }

        static void ReadOrders()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference("order");
            table.CreateIfNotExists();

            var query = new TableQuery<OrderEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Truck"));
            var orders = table.ExecuteQuery(query);
            Console.WriteLine("List of orders:");

            foreach (var order in orders)
            {
                Console.WriteLine("{0} - {1}", order.PartitionKey, order.RowKey);
            }

            //var orders = query.Execute()


            Console.ReadLine();

        }

        static void CreateBlob()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("signin");
            container.CreateIfNotExists();

            var blobName = String.Format("{0}_SignIn_{1:ddmmyyyss}.docx", 111, DateTime.UtcNow);

            var blob = container.GetBlockBlobReference(blobName);

            using (var fileStream = System.IO.File.OpenRead(@"F:\Mod07\Labfiles\Starter\sample.txt"))
            {
                blob.UploadFromStream(fileStream);
            }
            Console.WriteLine(blob.Uri);
        }

        static void ReadBlob()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("signin");
            container.CreateIfNotExists();

            //var blobName = String.Format("{0}_SignIn_{1:ddmmyyyss}.docx", 111, DateTime.UtcNow);

            var blob = container.GetBlockBlobReference("111_SignIn_1648201744.docx");

            var stream = blob.OpenRead();
            Console.WriteLine(blob.Uri);
            
        }

        static void ReadPrivateBlob()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("signin");
            container.CreateIfNotExists();

            var policy = new SharedAccessBlobPolicy();
            policy.SharedAccessExpiryTime = DateTime.Now.AddMinutes(15);
            policy.Permissions = SharedAccessBlobPermissions.Read;


            BlobContainerPermissions blobPermissions = new BlobContainerPermissions();
            blobPermissions.SharedAccessPolicies.Add("ReadPolicy", policy);
            blobPermissions.PublicAccess = BlobContainerPublicAccessType.Off;

            container.SetPermissions(blobPermissions);


            string sasToken = container.GetSharedAccessSignature(
                new SharedAccessBlobPolicy(), "ReadPolicy");

            //var blobName = String.Format("{0}_SignIn_{1:ddmmyyyss}.docx", 111, DateTime.UtcNow);

            var blob = container.GetBlockBlobReference("111_SignIn_1648201744.docx");

            var stream = blob.OpenRead();
            Console.WriteLine(blob.Uri + sasToken);

        }
    }

    public class OrderEntity : TableEntity
    {
        public OrderEntity(string category, string product)
        {
            PartitionKey = category;
            RowKey = product;
        }

        public OrderEntity()
        {

        }

        public string Description { get; set; }
        public string sku { get; set; }
    }
}
