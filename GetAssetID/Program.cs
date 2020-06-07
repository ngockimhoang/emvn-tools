using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetAssetID
{
    class LavilleTrack
    {        
        public string Catalog { get; set; }
        public string AlbumID { get; set; }
        public string TrackID { get; set; }
        public string CustomID
        {
            get
            {
                return string.Format("{0}_{1}_{2}", this.Catalog.ToLower(), this.AlbumID, this.TrackID);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var youtubeReportPath = @"C:\Users\kimhoang\Desktop\EMVN\emvn-cms-prod\asset_full_report_Audiomachine_L_v1-1.csv";
            var lavilleReportPath = @"C:\Users\kimhoang\Desktop\EMVN\emvn-cms-prod\laville_removal.csv";
            var outputPath = @"C:\Users\kimhoang\Desktop\EMVN\emvn-cms-prod\laville_yt_assets.csv";
            var lavilleTracks = new List<LavilleTrack>();
            var ytAssets = new List<YoutubeAsset>();
            using (var streamReader = System.IO.File.OpenText(lavilleReportPath))
            {
                using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    reader.Read();
                    reader.ReadHeader();
                    while (reader.Read())
                    {
                        var track = new LavilleTrack()
                        {
                            Catalog = reader.GetField<string>("Catalogue ID"),
                            AlbumID = reader.GetField<string>("Album ID#"),
                            TrackID = reader.GetField<string>("Track ID#"),
                        };
                        lavilleTracks.Add(track);
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
                            var customID = reader.GetField<string>("custom_id").Trim().ToLower();
                            var title = reader.GetField<string>("asset_title").Trim().ToLower();
                            if (customID.StartsWith("apl"))
                            {
                                if (lavilleTracks.Where(p => p.CustomID == customID).Any())
                                {                                    
                                    var ytAsset = new YoutubeAsset()
                                    {
                                        AssetID = reader.GetField<string>("asset_id"),
                                        AssetTitle = reader.GetField<string>("asset_title")
                                    };
                                    ytAssets.Add(ytAsset);
                                }

                            }
                            else if (title.StartsWith("apl"))
                            {
                                if (lavilleTracks.Where(p => p.CustomID == title).Any())
                                {
                                    var ytAsset = new YoutubeAsset()
                                    {
                                        AssetID = reader.GetField<string>("asset_id"),
                                        AssetTitle = reader.GetField<string>("asset_title")
                                    };
                                    ytAssets.Add(ytAsset);
                                }
                            }
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
                        csvWriter.WriteField<string>("Asset_ID");
                        csvWriter.WriteField<string>("Song_Title");
                        csvWriter.NextRecord();

                        foreach (var asset in ytAssets)
                        {
                            csvWriter.WriteField<string>(asset.AssetID);
                            csvWriter.WriteField<string>(asset.AssetTitle);
                            csvWriter.NextRecord();
                        }
                        csvWriter.Flush();
                    }
                }
            }
        }
    }
}
