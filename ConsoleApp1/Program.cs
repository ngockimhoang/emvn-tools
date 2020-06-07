using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"C:\Users\kimhoang\Desktop\EMVN\asset_full_report_Audiomachine_L_v1-1.csv";
            var outputPath = @"C:\Users\kimhoang\Desktop\EMVN\invalid_upc_albums.csv";
            var albums = new List<Album>();

            var regex = new System.Text.RegularExpressions.Regex(@"\b\d+\b");

            using (var streamReader = System.IO.File.OpenText(path))
            {
                using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    reader.Read();
                    reader.ReadHeader();
                    while (reader.Read())
                    {
                        var upc = reader.GetField<string>("upc");
                        var album = reader.GetField<string>("album");
                        var label = reader.GetField<string>("label");
                        if (!string.IsNullOrEmpty(album)
                            && (label.ToLower().Contains("audiomachine") || label.ToLower().Contains("apollo"))
                            && !regex.IsMatch(upc)
                            && !albums.Where(p => p.Name == album).Any())
                        {
                            albums.Add(new Album()
                            {
                                Name = album,
                                Label = label,
                                Upc = upc
                            });
                        }
                    }
                }
            }

            using (var streamWriter = new System.IO.StreamWriter(outputPath))
            {
                using (var writer = new CsvHelper.CsvWriter(streamWriter, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    writer.WriteHeader(typeof(Album));
                    writer.NextRecord();
                    foreach (var album in albums.OrderBy(p => p.Label).ThenBy(p => p.Name))
                    {
                        writer.WriteRecord<Album>(album);
                        writer.NextRecord();
                    }
                    writer.Flush();
                }
            }
        }
    }

    class Album
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Upc { get; set; }
    }
}
