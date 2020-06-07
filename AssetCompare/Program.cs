using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetCompare
{
    class Program
    {
        static void Main(string[] args)
        {            
            var youtubeReportPath = @"D:\GoProjects\src\emvn-minions\youtube-asset-cli\input\asset_full_report_Audiomachine_L_v1-1.csv";            
            var referenceReportPath = @"D:\GoProjects\src\emvn-minions\youtube-asset-cli\input\YouTube_Audiomachine_F_20200514.csv";
            var outputPath1 = @"C:\Users\kimhoang\Desktop\EMVN\apl-tracks-delete.csv";
            var outputPath2 = @"C:\Users\kimhoang\Desktop\EMVN\apl-tracks-review.csv";

            var headerRecord = new string[0];
            var aplTracksDelete = new List<string[]>();
            var aplTracksReview = new List<string[]>();
            var referenceDict = new Dictionary<string, int>();

            using (var streamReader = System.IO.File.OpenText(referenceReportPath))
            {
                using (var reader = new CsvHelper.CsvParser(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    reader.Read();
                    string[] row = null;
                    while ((row = reader.Read()) != null)
                    {
                        var reference = row[0].Trim();
                        var duration = Convert.ToInt32(row[14]);
                        referenceDict.Add(reference, duration);
                    }
                }
            }

            using (var streamReader = System.IO.File.OpenText(youtubeReportPath))
            {
                using (var reader = new CsvHelper.CsvParser(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    var headers = reader.Read().ToList();
                    headers.Add("length_sec");
                    headerRecord = headers.ToArray();
                    string[] asset = null;
                    while ((asset = reader.Read()) != null)
                    {
                        var label = asset[23].Trim().ToLower();
                        if (label == "apollo live")
                        {
                            var reference = asset[14].Trim().Split(' ')[0];
                            if (!string.IsNullOrEmpty(reference))
                            {
                                var activeClaims = Convert.ToInt32(asset[24]);
                                var duration = referenceDict[reference];
                                if (duration < 60)
                                {
                                    var row = asset.ToList();
                                    row.Add(duration.ToString());
                                    asset = row.ToArray();
                                    if (activeClaims > 10)
                                        aplTracksReview.Add(asset);
                                    else aplTracksDelete.Add(asset);
                                }
                            }
                        }
                    }
                }
            }

            using (var stream = new FileStream(outputPath1, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
                    {                        
                        foreach (var header in headerRecord)
                        {
                            csvWriter.WriteField<string>(header);
                        }                            
                        csvWriter.NextRecord();
                        
                        foreach (var asset in aplTracksDelete)
                        {
                            foreach (var field in asset)
                            {
                                csvWriter.WriteField<string>(field);
                            }
                            csvWriter.NextRecord();
                        }

                        csvWriter.Flush();
                    }
                }
            }

            using (var stream = new FileStream(outputPath2, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
                    {
                        foreach (var header in headerRecord)
                        {
                            csvWriter.WriteField<string>(header);
                        }
                        csvWriter.NextRecord();

                        foreach (var asset in aplTracksReview)
                        {
                            foreach (var field in asset)
                            {
                                csvWriter.WriteField<string>(field);
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
