using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace PartnerAlbum
{
    class Program
    {
        static void Main(string[] args)
        {
            var directory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "albums");
            System.IO.Directory.CreateDirectory(directory);
            var albums = GetAllAlbums();
            foreach (var album in albums)
            {
                Console.WriteLine("Getting ddex for album " + album.AlbumCode);
                SaveAlbumDDEXFile(directory, album);
            }           
        }

        static List<PartnerAlnum> GetAllAlbums()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://developers.emvn.co");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJUalhiaGFaeDk0WVc4bDh5MzY2WjNSZG4zbGF1Nkg1QSJ9.e7fr69iaYk0V2jl2OR3upYXm7_Sv5BDLufeGVcMlkVw");
                var response = client.GetAsync("/partner/album-list?pageSize=12000&pageNo=0").Result;
                return JsonConvert.DeserializeObject<List<PartnerAlnum>>(response.Content.ReadAsStringAsync().Result);
            }
        }

        static void SaveAlbumDDEXFile(string directory, PartnerAlnum album)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://developers.emvn.co");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJUalhiaGFaeDk0WVc4bDh5MzY2WjNSZG4zbGF1Nkg1QSJ9.e7fr69iaYk0V2jl2OR3upYXm7_Sv5BDLufeGVcMlkVw");
                var response = client.GetAsync("/partner/album/ddex/" + album.AlbumCode).Result;
                var content = response.Content.ReadAsStringAsync().Result;
                content = Regex.Unescape(content).Trim('\n').Trim('"');
                content = FormatXml(content);

                System.IO.File.WriteAllText(System.IO.Path.Combine(directory, album.AlbumCode + ".xml"), content);
            }
        }

        static string FormatXml(string xml)
        {
            try
            {
                XDocument doc = XDocument.Parse(xml);
                return doc.ToString();
            }
            catch (Exception e)
            {
                // Handle and throw if fatal exception here; don't just ignore them
                return xml;
            }
        }
    }

    class PartnerAlnum
    {
        [JsonProperty("album_code")]
        public string AlbumCode { get; set; }
        [JsonProperty("album_title")]
        public string AlbumTitle { get; set; }
        [JsonProperty("album_artist")]
        public string AlbumArtist { get; set; }
        [JsonProperty("catalog")]
        public string Catalog { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("upc")]
        public string UPC { get; set; }
        [JsonProperty("grid")]
        public string Grid { get; set; }
    }
}
