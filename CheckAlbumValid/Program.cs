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
            var ytAssetFile = @"D:\GoProjects\src\emvn-minions\youtube-asset-cli\input\asset_full_report_Audiomachine_L_v1-1.csv";
            var srDeletePath = @"C:\Users\kimhoang\Desktop\EMVN\sr-delete.csv";                        

            var assetDict = new Dictionary<string, YoutubeAsset>();            
            var takeDownSRAssetList = new List<string>();                                    

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
                            var numberOfDailyViews = reader.GetField<int>("approx_daily_views");
                            var assetID = reader.GetField<string>("asset_id");
                            assetDict.Add(assetID, new YoutubeAsset()
                            {
                                AssetID = assetID,
                                NumberOfActiveClaims = numberOfActiveClaims,
                                NumberOfDailyViews = numberOfDailyViews
                            });
                        }
                    }
                }
            }          

            foreach (var file in Directory.GetFiles(albumFoldersPath, "*.json", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(file);
                if (fileName == "APL 057.json")
                {
                    var cmsAlbum = JsonConvert.DeserializeObject<CmsAlbum>(File.ReadAllText(file));
                    var hasDuplicate = false;
                    foreach (var dupGroup in cmsAlbum.Assets.GroupBy(p => p.TrackCode)
                                                            .Where(p => p.Count() > 1))
                    {
                        hasDuplicate = true;
                        for (var i = 0; i < dupGroup.Count(); i++)
                        {
                            var asset = dupGroup.ToArray()[i];
                            var ytAsset = assetDict[asset.AssetID];
                            asset.NumberOfActiveClaims = ytAsset.NumberOfActiveClaims;
                            asset.NumberOfDailyViews = ytAsset.NumberOfDailyViews;
                        }

                        var dupGroupAssets = dupGroup.OrderByDescending(p => p.NumberOfActiveClaims).ThenByDescending(p => p.NumberOfDailyViews).ToArray();
                        for (var i = 1; i < dupGroupAssets.Count(); i++)
                        {
                            cmsAlbum.Assets.Remove(dupGroupAssets[i]);
                            takeDownSRAssetList.Add(dupGroupAssets[i].AssetID);
                        }                                         
                    }

                    if (hasDuplicate)
                    {
                        File.WriteAllText(file, JsonConvert.SerializeObject(cmsAlbum, Formatting.Indented));
                    }
                }
            }

            using (var stream = new FileStream(srDeletePath, FileMode.Create))
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
        }
    }
}
