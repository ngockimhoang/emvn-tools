using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using MongoDB.Bson;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"D:\emvn\google-service-accounts";
            var cmsYoutubeCredentials = new List<CmsYoutubeCredential>();
            foreach (var dir in Directory.GetDirectories(path, "emvn*"))
            {
                foreach (var keyFile in Directory.GetFiles(dir))
                {
                    var cmsYoutubeCredential = new CmsYoutubeCredential()
                    {
                        ID = new MongoID() { OID = ObjectId.GenerateNewId().ToString() },
                        ContentOwnerID = "Q8P88lEynMp6AHAV5RQNDQ",
                        Name = Path.GetFileName(dir),
                        Enabled = true,
                        Credential = File.ReadAllText(keyFile),
                        Type = "api",
                        IsNew = true
                    };
                    cmsYoutubeCredentials.Add(cmsYoutubeCredential);
                }
            }

            File.WriteAllText(path + @"\output_pretty.json", JsonConvert.SerializeObject(cmsYoutubeCredentials, Formatting.Indented));
            File.WriteAllText(path + @"\output.json", JsonConvert.SerializeObject(cmsYoutubeCredentials));
        }
    }

    class CmsYoutubeCredential
    {
        [JsonProperty("_id")]
        public MongoID ID { get; set; }
        [JsonProperty("content_owner_id")]
        public string ContentOwnerID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("credential")]
        public string Credential { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("is_new")]
        public bool IsNew { get; set; }
    }

    class MongoID
    {
        [JsonProperty("$oid")]
        public string OID { get; set; }
    }
}
