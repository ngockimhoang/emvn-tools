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
            var youtubeReportPath = @"C:\Users\kimhoang\Desktop\EMVN\asset_full_report_Audiomachine_L_v1-1.csv";            
            var outputPath = @"C:\Users\kimhoang\Desktop\EMVN\sr-delete.csv";            
            var ytAssets = new List<YoutubeAsset>();           

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
                            var label = reader.GetField<string>("asset_label").Trim();
                            if (label == "Minim"
                                || label == "Lovely Music Library"
                                || label == "Gothic Storm's Toolworks"
                                || label == "Sham Stalin"
                                || label == "WeWe"
                                || label == "Shin Hong Vinh"
                                || label == "Cuong Seven")
                            {
                                ytAssets.Add(new YoutubeAsset()
                                {
                                    AssetID = reader.GetField<string>("asset_id").Trim()
                                });
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
                        csvWriter.NextRecord();

                        foreach (var asset in ytAssets)
                        {
                            csvWriter.WriteField<string>(asset.AssetID);
                            csvWriter.NextRecord();
                        }
                        csvWriter.Flush();
                    }
                }
            }
        }
    }
}
