using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetAssetID
{    
    class Program
    {
        static void Main(string[] args)
        {
            var youtubeReportPath = @"D:\go-src\src\emvn-minions\youtube-asset-cli\input\asset_full_report_Audiomachine_Publishing_L_v1-2.csv";            
            var outputPath = @"D:\go-src\src\emvn-minions\youtube-asset-cli\output\twisted-jukebox-missing-label-assets.csv";
            var headers = new string[0];
            var assets = new List<string[]>();

            using (var streamReader = System.IO.File.OpenText(youtubeReportPath))
            {
                using (var reader = new CsvHelper.CsvParser(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    headers = reader.Read();
                    string[] row = null;
                    while ((row = reader.Read()) != null)
                    {                       
                        var customID = row[6];
                        var assetLabel = row[12];
                        if (!assetLabel.Contains("Twisted Jukebox")
                            && (customID.StartsWith("TJ")
                                || customID.StartsWith("ALIVE")
                                || customID.StartsWith("ANARCH")
                                || customID.StartsWith("AdM")
                                || customID.StartsWith("SIMPLY")))
                        {
                            assets.Add(row);
                        }
                    }
                }
            }

            using (var stream = new FileStream(outputPath, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
                    {
                        foreach (var header in headers)
                        {
                            csvWriter.WriteField(header);
                        }
                        csvWriter.NextRecord();

                        foreach (var asset in assets)
                        {
                            foreach (var column in asset)
                            {
                                csvWriter.WriteField(column);
                            }                            
                            csvWriter.NextRecord();
                        }
                        csvWriter.Flush();
                    }
                }
            }
        }
    }
}
