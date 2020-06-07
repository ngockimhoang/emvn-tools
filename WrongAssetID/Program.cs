using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongAssetID
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmsAssetPath = @"C:\Users\kimhoang\Desktop\EMVN\emvn-cms-prod\cms_assets.json";
            var youtubeReportPath = @"C:\Users\kimhoang\Desktop\EMVN\emvn-cms-prod\asset_full_report_Audiomachine_L_v1-1.csv";
            var outputPath = @"C:\Users\kimhoang\Desktop\EMVN\emvn-cms-prod\cms_assets_asset_id_fixed.csv";
            var output2Path = @"C:\Users\kimhoang\Desktop\EMVN\emvn-cms-prod\cms_assets_asset_id_not_found.csv";

            var cmsAssets = new List<CmsAsset>();
            using (var reader = new StreamReader(cmsAssetPath, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var cmsAsset = JsonConvert.DeserializeObject<CmsAsset>(line);
                    if ((string.IsNullOrEmpty(cmsAsset.AssetType) || cmsAsset.AssetType == "recording")
                        && !string.IsNullOrEmpty(cmsAsset.Status)
                        && (cmsAsset.Status.ToLower() == "success" || cmsAsset.Status.ToLower() == "warning")
                        && cmsAsset.AssetID != null
                        && cmsAsset.AssetID.Trim().Length == 11) //this is the length of reference id
                        cmsAssets.Add(cmsAsset);
                }
            }

            Console.WriteLine("Number of Assets with invalid asset id: {0}", cmsAssets.Count);


            var ytAssetsByReferenceID = new Dictionary<string, YoutubeAsset>();
            var ytAssetsByInactiveReferenceID = new Dictionary<string, YoutubeAsset>();
            using (var streamReader = System.IO.File.OpenText(youtubeReportPath))
            {
                using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    reader.Configuration.TrimOptions = CsvHelper.Configuration.TrimOptions.Trim;
                    reader.Read();
                    reader.ReadHeader();
                    while (reader.Read())
                    {
                        if (reader.GetField<string>("asset_type") == "SOUND_RECORDING")
                        {
                            var ytAsset = new YoutubeAsset()
                            {
                                ISRC = reader.GetField<string>("isrc"),
                                AssetID = reader.GetField<string>("asset_id"),
                                AssetTitle = reader.GetField<string>("asset_title"),
                                Artist = reader.GetField<string>("artist"),
                                AssetLabel = reader.GetField<string>("asset_label"),
                                ActiveReferenceID = reader.GetField<string>("active_reference_id"),
                                InactiveReferenceID = reader.GetField<string>("inactive_reference_id")
                            };
                            foreach (var activeReferenceID in ytAsset.ActiveReferenceIDList)
                            {
                                ytAssetsByReferenceID.Add(activeReferenceID, ytAsset);
                            }
                            foreach (var inactiveReferenceID in ytAsset.InactiveReferenceIDList)
                            {
                                ytAssetsByInactiveReferenceID.Add(inactiveReferenceID, ytAsset);
                            }                                                  
                        }
                    }
                }
            }

            var stream = new FileStream(outputPath, FileMode.Create);
            var stream2 = new FileStream(output2Path, FileMode.Create);
            var writer = new StreamWriter(stream, Encoding.UTF8);
            var writer2 = new StreamWriter(stream2, Encoding.UTF8);
            var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture);
            var csvWriter2 = new CsvHelper.CsvWriter(writer2, System.Threading.Thread.CurrentThread.CurrentCulture);

            csvWriter.Configuration.HasHeaderRecord = true;
            csvWriter.WriteField<string>("ID");
            csvWriter.WriteField<string>("Invalid_Asset_ID");
            csvWriter.WriteField<string>("Correct_Asset_ID");
            csvWriter.NextRecord();

            csvWriter2.Configuration.HasHeaderRecord = true;
            csvWriter2.WriteField<string>("ID");
            csvWriter2.WriteField<string>("Invalid_Asset_ID");
            csvWriter2.WriteField<string>("Correct_Asset_ID");
            csvWriter2.NextRecord();

            foreach (var cmsAsset in cmsAssets)
            {
                if (ytAssetsByReferenceID.ContainsKey(cmsAsset.AssetID))
                {
                    csvWriter.WriteField(cmsAsset.ID.OID);
                    csvWriter.WriteField(cmsAsset.AssetID);
                    csvWriter.WriteField(ytAssetsByReferenceID[cmsAsset.AssetID].AssetID);
                    csvWriter.NextRecord();
                }
                else if (ytAssetsByInactiveReferenceID.ContainsKey(cmsAsset.AssetID))
                {
                    csvWriter.WriteField(cmsAsset.ID.OID);
                    csvWriter.WriteField(cmsAsset.AssetID);
                    csvWriter.WriteField(ytAssetsByInactiveReferenceID[cmsAsset.AssetID].AssetID);
                    csvWriter.NextRecord();
                }
                else
                {
                    csvWriter2.WriteField(cmsAsset.ID.OID);
                    csvWriter2.WriteField(cmsAsset.AssetID);
                    csvWriter2.WriteField("");
                    csvWriter2.NextRecord();
                }
            }

            csvWriter.Flush();
            csvWriter2.Flush();
            csvWriter.Dispose();
            csvWriter2.Dispose();

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
