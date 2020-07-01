using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssertAsset
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputPath = @"C:\Users\kimhoang\Desktop\EMVN\03-23-20 Brand X Music asset report update.csv";
            var outputPath = @"C:\Users\kimhoang\Desktop\EMVN\assert_asset.csv";
            var assetList = new List<YoutubeAsset>();

            using (var streamReader = System.IO.File.OpenText(inputPath))
            {
                using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    reader.Read();
                    reader.ReadHeader();
                    while (reader.Read())
                    {
                        if (reader.GetField<string>("asset_type") == "SOUND_RECORDING")
                        {                            
                            var asset = new YoutubeAsset()
                            {
                                AssetID = reader.GetField<string>("asset_id"),
                                CustomID = reader.GetField<string>("custom_id"),
                                AssetTitle = reader.GetField<string>("asset_title"),
                                AssetLabel = "Brand X Music",

                            };
                            assetList.Add(asset);
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
                            csvWriter.WriteField(asset.AssetID);
                            csvWriter.WriteField(asset.CustomID);
                            csvWriter.WriteField("SOUND_RECORDING");
                            csvWriter.WriteField(asset.AssetTitle);
                            csvWriter.WriteField(asset.AssetLabel);
                            csvWriter.WriteField("WW");
                            csvWriter.WriteField("yes");
                            csvWriter.WriteField("");
                            csvWriter.WriteField("");
                            csvWriter.WriteField("Monetize in all countries");
                            csvWriter.WriteField("yes");
                            csvWriter.NextRecord();
                        }
                        csvWriter.Flush();
                    }
                }
            }
        }
    }
}
