using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportArtTrackUpdateCSV
{
    class Program
    {
        static void Main(string[] args)
        {
            var assetReport = @"D:\go-src\src\emvn-minions\youtube-asset-cli\input\asset_full_report_Audiomachine_L_v1-2.csv";            
            var output = @"D:\go-src\src\emvn-minions\youtube-asset-cli\output\art_track.csv";

            var assetMap = new Dictionary<string, List<YoutubeAsset>>();
            using (var streamReader = System.IO.File.OpenText(assetReport))
            {
                using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    reader.Read();
                    reader.ReadHeader();
                    while (reader.Read())
                    {
                        var assetType = reader.GetField("asset_type");
                        if (assetType.Equals("SOUND_RECORDING", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var customID = reader.GetField("custom_id");
                            var isrc = reader.GetField("isrc");
                            var grid = reader.GetField("grid");
                            var upc = reader.GetField("upc");
                            var artist = reader.GetField("artist");
                            var title = reader.GetField("asset_title");
                            var album = reader.GetField("album");
                            var label = reader.GetField("label");

                            var albumIdentifier =
                                !string.IsNullOrEmpty(grid) ? "grid_" + grid
                                : !string.IsNullOrEmpty(upc) ? "upc_" + upc
                                : null;
                            if (!string.IsNullOrEmpty(albumIdentifier))
                            {
                                var ytAsset = new YoutubeAsset()
                                {
                                    CustomID = customID,
                                    ISRC = isrc,
                                    GRID = grid,
                                    UPC = upc,
                                    Artist = artist,
                                    AssetTitle = title,
                                    Album = album,
                                    Label = label
                                };
                                List<YoutubeAsset> albumAssets = null;
                                if (!assetMap.ContainsKey(albumIdentifier))
                                {
                                    albumAssets = new List<YoutubeAsset>();
                                    assetMap.Add(albumIdentifier, albumAssets);
                                }
                                else
                                {
                                    albumAssets = assetMap[albumIdentifier];
                                }

                                albumAssets.Add(ytAsset);
                            }
                        }
                    }
                }
            }

            using (var outputStream = new FileStream(output, FileMode.Create))
            {
                using (var writer = new StreamWriter(outputStream, Encoding.UTF8))
                {
                    using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
                    {
                        csvWriter.WriteField("ddex_party_id");
                        csvWriter.WriteField("album_artist");
                        csvWriter.WriteField("album_artist_isnis");
                        csvWriter.WriteField("title");
                        csvWriter.WriteField("album_grid");
                        csvWriter.WriteField("album_ean");
                        csvWriter.WriteField("album_upc");
                        csvWriter.WriteField("album_release_date");
                        csvWriter.WriteField("album_label");
                        csvWriter.WriteField("album_art_filename");
                        csvWriter.WriteField("track_number");
                        csvWriter.WriteField("track_title");
                        csvWriter.WriteField("track_filename");
                        csvWriter.WriteField("track_custom_id");
                        csvWriter.WriteField("track_artist");
                        csvWriter.WriteField("track_artist_isnis");
                        csvWriter.WriteField("track_genres");
                        csvWriter.WriteField("track_isrc");
                        csvWriter.WriteField("track_pline");
                        csvWriter.WriteField("track_territory_start_dates");
                        csvWriter.WriteField("track_explicit_lyrics");
                        csvWriter.WriteField("track_add_at_asset_labels");
                        csvWriter.WriteField("track_add_sr_asset_labels");
                        csvWriter.NextRecord();

                        foreach (var albumAssets in assetMap)
                        {
                            //sort by track number
                            var sortedAssets = albumAssets.Value.OrderBy(p => p.TrackNumber).ToList();
                            var trackNumber = 0;
                            foreach (var asset in sortedAssets)
                            {
                                csvWriter.WriteField("");
                                csvWriter.WriteField(asset.Artist);
                                csvWriter.WriteField("");
                                csvWriter.WriteField(asset.Album);
                                csvWriter.WriteField(asset.GRID);
                                csvWriter.WriteField("");
                                csvWriter.WriteField(asset.UPC);
                                csvWriter.WriteField("");
                                csvWriter.WriteField(asset.Label);
                                csvWriter.WriteField("");
                                csvWriter.WriteField(++trackNumber);
                                csvWriter.WriteField(asset.AssetTitle);
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField(asset.Artist);
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField(asset.ISRC);
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.NextRecord();
                            }
                        }
                    }
                }
            }                                 
        }
    }
}
