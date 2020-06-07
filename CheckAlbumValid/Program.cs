using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Common;
using System.Xml.Serialization;

namespace CheckAlbumValid
{
    class Program
    {
        static void Main(string[] args)
        {
            var albumFoldersPath = @"D:\GoProjects\src\emvn-minions\youtube-asset-cli\output\album_tracks";
            var ddexPath = @"D:\GoProjects\src\emvn-minions\youtube-asset-cli\output\ddex_packages";
            var ytAssetFile = @"D:\GoProjects\src\emvn-minions\youtube-asset-cli\input\asset_full_report_Audiomachine_L_v1-1.csv";
            var srDeletePath = @"C:\Users\kimhoang\Desktop\EMVN\sr-delete.csv";
            var atDeletePath = @"C:\Users\kimhoang\Desktop\EMVN\at-delete.csv";
            var reviewPath = @"C:\Users\kimhoang\Desktop\EMVN\duplicate-review.csv";

            var assetDict = new Dictionary<string, YoutubeAsset>();
            var SRToATDict = new Dictionary<string, string>();
            var takeDownSRAssetList = new List<string>();
            var takeDownATAssetList = new List<string>();
            var takeDownRecentSRAssetList = new List<string>();
            var takeDownRecentATAssetList = new List<string>();
            var needReviewSRAssetList = new List<List<string>>();

            using (var streamReader = System.IO.File.OpenText(ytAssetFile))
            {
                using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    reader.Read();
                    reader.ReadHeader();
                    while (reader.Read())
                    {
                        var assetType = reader.GetField<string>("asset_type");
                        if (assetType == "SOUND_RECORDING")
                        {
                            var numberOfActiveClaims = reader.GetField<int>("active_claims");
                            var assetID = reader.GetField<string>("asset_id");
                            assetDict.Add(assetID, new YoutubeAsset()
                            {
                                AssetID = assetID,
                                NumberOfActiveClaims = numberOfActiveClaims
                            });
                        }
                    }
                }
            }

            foreach (var file in Directory.GetFiles(ddexPath, "ACK_*.xml", SearchOption.AllDirectories))
            {
                if (file.Contains("APL"))
                {
                    var xmlSerializer = new XmlSerializer(typeof(AckMessage));
                    using (var reader = File.OpenRead(file))
                    {
                        var ackMessage = xmlSerializer.Deserialize(reader) as AckMessage;
                        foreach (var affectedResource in ackMessage.AffectedResources)
                        {
                            var srProp = affectedResource.Properties.Where(p => p.Namespace == "YOUTUBE:SR_ASSET_ID").FirstOrDefault();
                            var atProp = affectedResource.Properties.Where(p => p.Namespace == "YOUTUBE:AT_ASSET_ID").FirstOrDefault();
                            if (srProp != null
                                && atProp != null)
                            {
                                SRToATDict.Add(srProp.Text, atProp.Text);
                            }
                        }
                    }
                }
            }

            foreach (var file in Directory.GetFiles(albumFoldersPath, "*.json", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(file);
                if (fileName.StartsWith("APL"))
                {
                    var cmsAlbum = JsonConvert.DeserializeObject<CmsAlbum>(File.ReadAllText(file));                                        
                    foreach (var dupGroup in cmsAlbum.Assets.GroupBy(p => p.TrackCode)
                                                            .Where(p => p.Count() > 1))
                    {                        
                        var removeCount = 0;
                        var remainingList = new List<string>();
                        for (var i = 0; i < dupGroup.Count(); i++)
                        {
                            var asset = dupGroup.ToArray()[i];
                            //it does not appear in the dict
                            //it means the this is added wrongly in recent packages
                            if (!assetDict.ContainsKey(asset.AssetID))
                            {
                                takeDownRecentSRAssetList.Add(asset.AssetID);
                                if (SRToATDict.ContainsKey(asset.AssetID))
                                {
                                    takeDownRecentATAssetList.Add(SRToATDict[asset.AssetID]);
                                }
                                removeCount++;
                            }
                            else
                            {
                                var numberOfClaims = assetDict[asset.AssetID].NumberOfActiveClaims;
                                if (numberOfClaims <= 0)
                                {
                                    takeDownSRAssetList.Add(asset.AssetID);
                                    if (SRToATDict.ContainsKey(asset.AssetID))
                                    {
                                        takeDownATAssetList.Add(SRToATDict[asset.AssetID]);
                                    }
                                    removeCount++;
                                }
                                else remainingList.Add(asset.AssetID);
                            }
                                                        
                            if (dupGroup.Count() - removeCount == 1)
                                break;
                        }

                        if (remainingList.Count >= 2)
                        {
                            needReviewSRAssetList.Add(remainingList);
                        }
                    }
                }                
            }       

            using (var stream = new FileStream(srDeletePath, FileMode.OpenOrCreate))
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
                    {
                        csvWriter.Configuration.HasHeaderRecord = true;
                        csvWriter.WriteField<string>("asset_id");                        
                        csvWriter.NextRecord();

                        foreach (var cmsAsset in takeDownSRAssetList)
                        {
                            csvWriter.WriteField(cmsAsset);                            
                            csvWriter.NextRecord();
                        }

                        csvWriter.Flush();
                    }
                }
            }

            using (var stream = new FileStream(atDeletePath, FileMode.OpenOrCreate))
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
                    {
                        csvWriter.Configuration.HasHeaderRecord = true;
                        csvWriter.WriteField<string>("asset_id");
                        csvWriter.NextRecord();

                        foreach (var cmsAsset in takeDownATAssetList)
                        {
                            csvWriter.WriteField(cmsAsset);
                            csvWriter.NextRecord();
                        }

                        csvWriter.Flush();
                    }
                }
            }

            using (var stream = new FileStream(reviewPath, FileMode.OpenOrCreate))
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    foreach (var assets in needReviewSRAssetList)
                    {
                        writer.WriteLine(string.Join(",", assets.ToArray()));
                    }
                    writer.Flush();
                }
            }
        }
    }
}
