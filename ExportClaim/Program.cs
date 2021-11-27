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
            var output = @"D:\go-src\src\emvn-minions\youtube-asset-cli\output\ACV-Entertainment-claims.csv";            

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
            csvWriter.WriteField("channel_id");
            csvWriter.WriteField("channel_display_name");
            csvWriter.WriteField("claim_created_date");
            csvWriter.WriteField("asset_title");
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
                        var assetTitle = reader.GetField("asset_title");
                        var assetID = reader.GetField("asset_id");
                        if (string.Equals(label, "ACV Entertainment", StringComparison.InvariantCultureIgnoreCase)
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
                                csvWriter.WriteField(channelID);
                                csvWriter.WriteField(channelName);
                                csvWriter.WriteField(claimCreatedDateStr);
                                csvWriter.WriteField(assetTitle);
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
