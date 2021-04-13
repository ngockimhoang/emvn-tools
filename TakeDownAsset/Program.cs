﻿using Common;
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
            var srDeletePath = @"D:\go-src\src\emvn-minions\youtube-asset-cli\input\tools\sr-delete.csv";            
            var youtubeReportPath = @"D:\go-src\src\emvn-minions\youtube-asset-cli\input\asset_full_report_Audiomachine_L_v1-2.csv";
            var referencePath = @"D:\go-src\src\emvn-minions\youtube-asset-cli\output\tools\take_down_reference.csv";
            var ownershipPath = @"D:\go-src\src\emvn-minions\youtube-asset-cli\output\tools\take_down_ownership.csv";
            var artTrackPath = @"D:\go-src\src\emvn-minions\youtube-asset-cli\output\tools\take_down_art_track.csv";
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
                            var isrc = reader.GetField<string>("isrc");
                            var grid = reader.GetField<string>("grid");
                            var constiuentAssetIDStr = reader.GetField<string>("constituent_asset_id");
                            if (assetList.ContainsKey(assetID))
                            {
                                var asset = assetList[assetID];
                                if (asset != null)
                                {
                                    asset.ActiveReferenceID = activeReference;
                                    asset.ISRC = isrc;
                                    asset.GRID = grid;
                                }
                            }
                            else if (!string.IsNullOrEmpty(constiuentAssetIDStr))
                            {
                                //check constiuentAssetID                              
                                var constiuentAssetIDList = constiuentAssetIDStr.Split(' ').Where(p => !string.IsNullOrEmpty(p)).Select(p => p.Trim()).ToArray();
                                foreach (var constiuentAssetID in constiuentAssetIDList)
                                {
                                    if (assetList.ContainsKey(constiuentAssetID))
                                    {
                                        //this case here means asset id to be deleted is a part of merged asset 
                                        //so we remove from the dictionary and add asset with main asset id
                                        var asset = assetList[constiuentAssetID];
                                        if (asset != null)
                                        {
                                            assetList.Remove(constiuentAssetID);

                                            asset.ActiveReferenceID = activeReference;
                                            asset.ISRC = isrc;
                                            asset.GRID = grid;
                                            assetList.Add(assetID, asset);
                                        }
                                        break;
                                    }
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

            using (var stream = new FileStream(artTrackPath, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
                    {
                        csvWriter.Configuration.HasHeaderRecord = true;
                        csvWriter.WriteField<string>("ddex_party_id");
                        csvWriter.WriteField<string>("album_artist");
                        csvWriter.WriteField<string>("album_artist_isnis");
                        csvWriter.WriteField<string>("album_title");
                        csvWriter.WriteField<string>("album_grid");
                        csvWriter.WriteField<string>("album_ean");
                        csvWriter.WriteField<string>("album_upc");
                        csvWriter.WriteField<string>("album_release_date");
                        csvWriter.WriteField<string>("album_label");
                        csvWriter.WriteField<string>("album_art_filename");
                        csvWriter.WriteField<string>("track_number");
                        csvWriter.WriteField<string>("track_title");
                        csvWriter.WriteField<string>("track_filename");
                        csvWriter.WriteField<string>("track_custom_id");
                        csvWriter.WriteField<string>("track_artist");
                        csvWriter.WriteField<string>("track_artist_isnis");
                        csvWriter.WriteField<string>("track_genres");
                        csvWriter.WriteField<string>("track_isrc");
                        csvWriter.WriteField<string>("track_pline");
                        csvWriter.WriteField<string>("track_territory_start_dates");
                        csvWriter.WriteField<string>("track_explicit_lyrics");
                        csvWriter.WriteField<string>("track_add_at_asset_labelss");
                        csvWriter.WriteField<string>("track_add_sr_asset_labels");                        
                        csvWriter.NextRecord();

                        foreach (var asset in assetList)
                        {             
                            if (!string.IsNullOrEmpty(asset.Value.GRID)
                                && !string.IsNullOrEmpty(asset.Value.ISRC))
                            {
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>(asset.Value.GRID);
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>(asset.Value.ISRC);
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.WriteField<string>("");
                                csvWriter.NextRecord();
                            }                            
                        }
                        csvWriter.Flush();
                    }
                }
            }
        }
    }
}
