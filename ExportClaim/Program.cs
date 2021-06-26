using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportClaim
{
    class Program
    {
        static void Main(string[] args)
        {
            var claimReport = @"D:\go-src\src\emvn-minions\youtube-asset-cli\input\claim_report_Audiomachine_C.csv";
            var unofficialChannelFile = @"D:\go-src\src\emvn-minions\youtube-asset-cli\input\unofficial-channels.json";
            //var assetReport = @"D:\go-src\src\emvn-minions\youtube-asset-cli\input\asset_full_report_Audiomachine_L_v1-2.csv";
            var output = @"D:\go-src\src\emvn-minions\youtube-asset-cli\output\stye-claims.csv";
            //var styeAssets = new Dictionary<string, string>();
            //using (var streamReader = System.IO.File.OpenText(assetReport))
            //{
            //    using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
            //    {
            //        reader.ReadHeader();
            //        while (reader.Read())
            //        {
            //            var assetID = reader.GetField("asset_id");
            //            var assetType = reader.GetField("asset_type");
            //            var label = reader.GetField("asset_label");
            //            var constituent_asset_id = reader.GetField("constituent_asset_id");
            //            var childAssetIDs = constituent_asset_id.Split(' ');
            //            if (assetType.ToUpper() == "SOUND_RECORDING"
            //                && label.ToUpper().Contains("SONGS TO YOUR EYES"))
            //            {
            //                if (!styeAssets.ContainsKey(assetID))
            //                {
            //                    styeAssets.Add(assetID, assetID);
            //                }
            //                foreach (var childAssetID in childAssetIDs)
            //                {
            //                    if (!styeAssets.ContainsKey(childAssetID))
            //                    {
            //                        styeAssets.Add(childAssetID, childAssetID);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            var unofficialChannels = JsonConvert.DeserializeObject<ChannelList>(System.IO.File.ReadAllText(unofficialChannelFile));
            var unofficialChannelsDict = new Dictionary<string, string>();
            foreach (var channel in unofficialChannels.ListChannel)
            {
                if (!unofficialChannelsDict.ContainsKey(channel.ChannelID))
                    unofficialChannelsDict.Add(channel.ChannelID, channel.ChannelID);
            }

            var outputStream = new FileStream(output, FileMode.Create);
            var writer = new StreamWriter(outputStream, Encoding.UTF8);
            var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture);
            csvWriter.WriteField("claim_id");
            csvWriter.WriteField("video_id");
            csvWriter.WriteField("video_title");
            csvWriter.WriteField("channel_display_name");
            csvWriter.WriteField("claim_created_date");
            csvWriter.NextRecord();
            using (var streamReader = System.IO.File.OpenText(claimReport))
            {
                using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    reader.Read();
                    reader.ReadHeader();
                    while (reader.Read())
                    {
                        var claimID = reader.GetField("claim_id");                        
                        var videoID = reader.GetField("video_id");
                        var channelID = reader.GetField("channel_id");
                        var channelName = reader.GetField("channel_display_name");
                        var videoTitle = reader.GetField("video_title");
                        var claimCreatedDateStr = reader.GetField("claim_created_date");
                        var label = reader.GetField("asset_labels");
                        if (label.ToUpper().Contains("SONGS TO YOUR EYES")
                            && !string.IsNullOrEmpty(channelName)
                            && !string.IsNullOrEmpty(videoTitle)
                            && !string.IsNullOrEmpty(claimCreatedDateStr)
                            && !unofficialChannelsDict.ContainsKey(channelID))
                        {
                            var claimCreatedDate = DateTime.ParseExact(claimCreatedDateStr, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                            if (claimCreatedDate.Year >= 2018)
                            {
                                csvWriter.WriteField(claimID);
                                csvWriter.WriteField(videoID);
                                csvWriter.WriteField(videoTitle);
                                csvWriter.WriteField(channelName);
                                csvWriter.WriteField(claimCreatedDateStr);
                                csvWriter.NextRecord();
                            }
                        }
                    }
                }
            }

            csvWriter.Flush();
            csvWriter.Dispose();
            writer.Dispose();
            outputStream.Dispose();
        }        
    }

    public class Channel
    {
        [JsonProperty("channelId")]
        public string ChannelID { get; set; }
        [JsonProperty("channelName")]
        public string ChannelName { get; set; }
    }

    public class ChannelList
    {
        [JsonProperty("listChannel")]
        public Channel[] ListChannel { get; set; }
    }
}
