using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CmsAsset
    {
        [JsonProperty("asset_id")]
        public string AssetID { get; set; }
        [JsonProperty("art_track_asset_id")]
        public string ArtTrackAssetID { get; set; }
        [JsonProperty("video_id")]
        public string VideoID { get; set; }
        [JsonProperty("custom_id")]
        public string CustomID { get; set; }
        [JsonProperty("artist")]
        public string Artist { get; set; }
        [JsonProperty("genre")]
        public string Genre { get; set; }
        [JsonProperty("isrc")]
        public string ISRC { get; set; }
        [JsonProperty("song_title")]
        public string SongTitle { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("youtube_label")]
        public string YoutubeLabel { get; set; }
        [JsonProperty("duration")]
        public ulong Duration { get; set; }
        [JsonProperty("filename")]
        public string Filename { get; set; }
        [JsonProperty("track_code")]
        public int TrackCode { get; set; }
        [JsonProperty("original_song_title")]
        public string OriginalSongTitle { get; set; }
        [JsonProperty("writers")]
        public List<AssetWriter> Writers { get; set; }
        [JsonProperty("publishers")]
        public List<AssetPublisher> Publishers { get; set; }
        [JsonIgnore]
        public int NumberOfActiveClaims { get; set; }
        [JsonIgnore]
        public int NumberOfDailyViews { get; set; }

        #region External Properties
        [JsonIgnore]
        public string NewFilePath { get; set; }
        #endregion
    }

    public class AssetWriter
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class AssetPublisher
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("sync_ownership_share")]
        public int SyncOwnershipShare { get; set; }
        [JsonProperty("sync_ownership_territory")]
        public string SyncOwnershipTerritory { get; set; }
        [JsonProperty("sync_ownership_restriction")]
        public string SyncOwnershipRestriction { get; set; }
        [JsonProperty("mechanical_ownership_share")]
        public int MechanicalOwnershipShare { get; set; }
        [JsonProperty("mechanical_ownership_territory")]
        public string MechanicalOwnershipTerritory { get; set; }
        [JsonProperty("mechanical_ownership_restriction")]
        public string MechanicalOwnershipRestriction { get; set; }
        [JsonProperty("performance_ownership_share")]
        public int PerformanceOwnershipShare { get; set; }
        [JsonProperty("performance_ownership_territory")]
        public string PerformanceOwnershipTerritory { get; set; }
        [JsonProperty("performance_ownership_restriction")]
        public string PerformanceOwnershipRestriction { get; set; }
    }

    public class MongoID
    {
        [JsonProperty("$oid")]
        public string OID { get; set; }
    }


    public class YoutubeAsset
    {
        public string ISRC { get; set; }
        public string AssetID { get; set; }
        public string AssetTitle { get; set; }
        public string Artist { get; set; }
        public string AssetLabel { get; set; }
        public string ActiveReferenceID { get; set; }
        public string InactiveReferenceID { get; set; }
        public int NumberOfActiveClaims { get; set; }
        public int NumberOfDailyViews { get; set; }
        public string[] ActiveReferenceIDList
        {
            get
            {
                if (ActiveReferenceID != null)
                    return ActiveReferenceID.Split(' ').Where(p => !string.IsNullOrEmpty(p)).Select(p => p.Trim()).ToArray();
                else return new string[0];
            }
        }

        public string[] InactiveReferenceIDList
        {
            get
            {
                if (InactiveReferenceID != null)
                    return InactiveReferenceID.Split(' ').Where(p => !string.IsNullOrEmpty(p)).Select(p => p.Trim()).ToArray();
                else return new string[0];
            }
        }
    }
}
