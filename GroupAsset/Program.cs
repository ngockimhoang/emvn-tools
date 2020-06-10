using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupAsset
{
    class Program
    {
        static void Main(string[] args)
        {
            var youtubeReportPath = @"C:\Users\kim.hoang\Desktop\emvn\asset_full_report_Audiomachine_L_v1-1.csv";
            var outputPath = @"C:\Users\kim.hoang\Desktop\emvn\asset-count-by-label.csv";

            var assets = new List<YoutubeAsset>();
            using (var streamReader = System.IO.File.OpenText(youtubeReportPath))
            {                
                using (var reader = new CsvHelper.CsvParser(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    reader.Read();
                    string[] asset = null;
                    while ((asset = reader.Read()) != null)
                    {
                        var assetType = asset[2].Trim();
                        var label = asset[23].Trim();
                        var reference = asset[14].Trim();
                        if (label.Contains("Audiomachine") || label.Contains("Apollo Live") || label.Contains("APL Publishing") || string.IsNullOrEmpty(reference) || assetType != "SOUND_RECORDING")
                            continue;

                        assets.Add(new YoutubeAsset()
                        {
                            AssetID = asset[1],
                            AssetTitle = asset[10],
                            AssetLabel = label
                        });
                    }
                }
            }            

            using (var stream = new FileStream(outputPath, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
                    {
                        csvWriter.WriteField<string>("Label");
                        csvWriter.WriteField<string>("Count");
                        csvWriter.NextRecord();

                        foreach (var label in assets.GroupBy(p => p.AssetLabel)
                                                    .OrderByDescending(p => p.Count())
                                                    .Select(p => new { Label = p.Key, Count = p.Count() }))
                        {
                            csvWriter.WriteField<string>(label.Label);
                            csvWriter.WriteField<int>(label.Count);
                            csvWriter.NextRecord();
                        }

                        csvWriter.Flush();
                    }
                }
            }
        }
    }
}
