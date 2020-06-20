using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeDownAsset
{
    class Program
    {
        static void Main(string[] args)
        {
            var srDeletePath = @"C:\Users\kimhoang\Desktop\EMVN\sr-delete.csv";            
            var youtubeReportPath = @"D:\GoProjects\src\emvn-minions\youtube-asset-cli\input\asset_full_report_Audiomachine_L_v1-1.csv";
            var referencePath = @"C:\Users\kimhoang\Desktop\EMVN\take_down_reference.csv";
            var ownershipPath = @"C:\Users\kimhoang\Desktop\EMVN\take_down_ownership.csv";            
            var assetList = new Dictionary<string, YoutubeAsset>();            

            using (var streamReader = System.IO.File.OpenText(srDeletePath))
            {
                using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    reader.Read();
                    reader.ReadHeader();
                    while (reader.Read())
                    {
                        var assetID = reader.GetField<string>(0);
                        if (!assetList.ContainsKey(assetID))
                        {
                            assetList.Add(assetID, new YoutubeAsset()
                            {
                                AssetID = assetID
                            });
                        }                  
                    }
                }
            }            

            using (var streamReader = System.IO.File.OpenText(youtubeReportPath))
            {
                using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    reader.Read();
                    reader.ReadHeader();
                    while (reader.Read())
                    {
                        if (reader.GetField<string>("asset_type") == "SOUND_RECORDING")
                        {
                            var assetID = reader.GetField<string>("asset_id");
                            var activeReference = reader.GetField<string>("active_reference_id");
                            if (assetList.ContainsKey(assetID))
                            {
                                var asset = assetList[assetID];
                                if (asset != null)
                                {
                                    asset.ActiveReferenceID = activeReference;
                                }
                            }
                        }
                    }
                }
            }

            using (var stream = new FileStream(referencePath, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
                    {
                        csvWriter.Configuration.HasHeaderRecord = true;
                        csvWriter.WriteField<string>("reference_id");
                        csvWriter.WriteField<string>("reference_state");
                        csvWriter.WriteField<string>("release_claims");
                        csvWriter.NextRecord();

                        foreach (var asset in assetList)
                        {
                            foreach (var reference in asset.Value.ActiveReferenceIDList)
                            {
                                csvWriter.WriteField(reference);
                                csvWriter.WriteField("deactivate");
                                csvWriter.WriteField("");
                                csvWriter.NextRecord();
                            }
                        }
                        csvWriter.Flush();
                    }
                }
            }

            using (var stream = new FileStream(ownershipPath, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
                    {
                        csvWriter.Configuration.HasHeaderRecord = true;
                        csvWriter.WriteField<string>("asset_id");
                        csvWriter.WriteField<string>("custom_id");
                        csvWriter.WriteField<string>("asset_type");
                        csvWriter.WriteField<string>("title");
                        csvWriter.WriteField<string>("add_asset_labels");
                        csvWriter.WriteField<string>("ownership");
                        csvWriter.WriteField<string>("enable_content_id");
                        csvWriter.WriteField<string>("reference_filename");
                        csvWriter.WriteField<string>("reference_exclusions");
                        csvWriter.WriteField<string>("match_policy");
                        csvWriter.WriteField<string>("update_all_claims");
                        csvWriter.NextRecord();

                        foreach (var asset in assetList)
                        {
                            csvWriter.WriteField(asset.Key);
                            csvWriter.WriteField("");
                            csvWriter.WriteField("SOUND_RECORDING");
                            csvWriter.WriteField("");
                            csvWriter.WriteField("");
                            csvWriter.WriteField("0%WW");
                            csvWriter.WriteField("");
                            csvWriter.WriteField("");
                            csvWriter.WriteField("");
                            csvWriter.WriteField("");
                            csvWriter.WriteField("");
                            csvWriter.NextRecord();
                        }                       
                        csvWriter.Flush();
                    }
                }
            }            
        }
    }
}
