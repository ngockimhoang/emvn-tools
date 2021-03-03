using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Whitelist
{
    class Program
    {
        static void Main(string[] args)
        {
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJxRlFxUU1PdWFFUnR2NDQ4TjBQT1BwT3JNa1U2dDcyQSJ9.D3L4zv6TdHDneTG1k7wjkMcrZLlgQQuUeBUuabiRw5Y";
            var inputPath = @"D:\go-src\src\emvn-minions\youtube-asset-cli\input\whitelist.csv";            

            using (var streamReader = System.IO.File.OpenText(inputPath))
            {
                using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    reader.Read();
                    reader.ReadHeader();
                    while (reader.Read())
                    {
                        var channelID = reader.GetField<string>(0);
                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            var request = new WhitelistCreateRequest()
                            {
                                ID = channelID
                            };
                            Console.WriteLine("Whitelisting channel " + channelID);
                            var result = client.PostAsync("https://developers.emvn.co/v2/youtube/whitelists", new StringContent(JsonConvert.SerializeObject(request))).Result;                       
                            Console.WriteLine("Whitelisting channel " + channelID + " result " + result.StatusCode);
                        }                      
                    }
                }
            }
        }

        class WhitelistCreateRequest
        {
            [JsonProperty("id")]
            public string ID { get; set; }
        }       
    }
}
