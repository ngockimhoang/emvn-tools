using Common;
using MongoDB.Driver;
using MongoDB.Bson;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixAssetID
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var inputPath = @"C:\Users\kimhoang\Desktop\EMVN\emvn-cms-prod\cms_assets_asset_id_fixed.csv";

            using (var streamReader = System.IO.File.OpenText(inputPath))
            {
                using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    //ConnectionInfo conn = new ConnectionInfo("159.69.4.21", 22, "kimhoang", new PrivateKeyAuthenticationMethod("kimhoang", new PrivateKeyFile(@"C:\Users\kimhoang\kim")));
                    //var client = new SshClient(conn);
                    //client.Connect();
                    //ForwardedPortLocal forwardedPortLocal = new ForwardedPortLocal("127.0.0.1", 5477, "10.42.176.218", 27017);
                    //client.AddForwardedPort(forwardedPortLocal);
                    //forwardedPortLocal.Start();

                    var mongoClient = new MongoClient("mongodb://localhost:27017/?readPreference=primary&ssl=false&uuidRepresentation=Standard");                    

                    var dbList = mongoClient.ListDatabases().ToList();
                    Console.WriteLine("The list of databases are:");
                    foreach (var item in dbList)
                    {
                        Console.WriteLine(item);
                    }


                    var emvnCmsDB = mongoClient.GetDatabase("emvn-cms");

                    var collections = emvnCmsDB.ListCollections().ToList();
                    
                    var collection = emvnCmsDB.GetCollection<BsonDocument>("cms-assets");
                    var test = collection.Find(new BsonDocument()).Limit(100).CountDocuments();
                    var filter = new BsonDocument();



                    //using (IAsyncCursor<BsonDocument> cursor = await collection.FindAsync(new BsonDocument()))
                    //{
                    //    while (await cursor.MoveNextAsync())
                    //    {
                    //        IEnumerable<BsonDocument> batch = cursor.Current;
                    //        foreach (BsonDocument document in batch)
                    //        {
                    //            Console.WriteLine(document);
                    //            Console.WriteLine();
                    //        }
                    //    }
                    //}

                    //client.Dispose();       

                    Console.ReadKey();
                }
            }
        }
    }
}
