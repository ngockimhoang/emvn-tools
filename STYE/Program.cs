using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STYE
{
    class Program
    {
        static void Main(string[] args)
        {
            //var originalMappingFile = @"D:\go-src\src\emvn-minions\youtube-asset-cli\input\STYE\STYE_EPIC ELITE ASSETS EXPORT - With Mapping.csv";
            //var finalMappingFile = @"D:\go-src\src\emvn-minions\youtube-asset-cli\input\STYE\STYE_EPIC ELITE ASSETS EXPORT WITH SA ID - STYE_EPIC ELITE ASSETS EXPORT ALL.csv";
            //var albumCodeFile = @"D:\go-src\src\emvn-minions\youtube-asset-cli\input\STYE\album_code.csv";

            //var saIDToAlbumCode = new Dictionary<string, string>();
            //var albumMap = new Dictionary<string, List<Tuple<string, string, string>>>();
            //using (var streamReader = System.IO.File.OpenText(albumCodeFile))
            //{
            //    using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
            //    {
            //        reader.Read();
            //        reader.ReadHeader();
            //        while (reader.Read())
            //        {
            //            var assetID = reader.GetField("Asset ID");
            //            var saID = reader.GetField("Source Audio ID");
            //            var albumCode = reader.GetField("Album code");
            //            if (!saIDToAlbumCode.ContainsKey(saID))
            //                saIDToAlbumCode.Add(saID, albumCode);
            //            List<Tuple<string, string, string>> album = null;
            //            if (albumMap.ContainsKey(albumCode))
            //            {
            //                album = albumMap[albumCode];
            //            }
            //            else
            //            {
            //                album = new List<Tuple<string, string, string>>();
            //                albumMap[albumCode] = album;
            //            }
            //            album.Add(new Tuple<string, string, string>(assetID, saID, albumCode));
            //        }
            //    }
            //}

            //var unmappedTracks = new Dictionary<string, string>();
            //using (var streamReader = System.IO.File.OpenText(originalMappingFile))
            //{
            //    using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
            //    {
            //        reader.Read();
            //        reader.ReadHeader();
            //        while (reader.Read())
            //        {
            //            if (string.IsNullOrEmpty(reader.GetField<string>("Source Audio ID")))
            //            {
            //                var assetID = reader.GetField<string>("Asset ID");
            //                unmappedTracks.Add(assetID, assetID);
            //            }
            //        }
            //    }
            //}

            ////asset id - sa id - album code
            //var finalTracks = new List<Tuple<string, string, string>>();
            //using (var streamReader = System.IO.File.OpenText(finalMappingFile))
            //{
            //    using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
            //    {
            //        reader.Read();
            //        reader.ReadHeader();
            //        while (reader.Read())
            //        {
            //            var assetID = reader.GetField("Asset ID");
            //            var saID = reader.GetField("Source Audio ID");
            //            var removedFromCID = reader.GetField("Removedfrom CID");
            //            if (unmappedTracks.ContainsKey(assetID)
            //                && removedFromCID.ToUpper() != "REMOVED")
            //            {
            //                var albumCode = saIDToAlbumCode[saID];
            //                var album = albumMap[albumCode];
            //                foreach (var track in album)
            //                {
            //                    finalTracks.Add(new Tuple<string, string, string>(track.Item1, track.Item2, track.Item3));
            //                }                            
            //            }
            //        }
            //    }
            //}

            //finalTracks = finalTracks.GroupBy(p => p.Item1)
            //                         .Select(g => g.First())
            //                         .ToList();

            ////output
            ////download-tracks.csv
            //using (var stream = new FileStream(@"D:\go-src\src\emvn-minions\youtube-asset-cli\input\STYE\download-tracks.csv", FileMode.Create))
            //{
            //    using (var writer = new StreamWriter(stream, Encoding.UTF8))
            //    {
            //        using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
            //        {
            //            csvWriter.Configuration.HasHeaderRecord = true;
            //            csvWriter.WriteField<string>("Source Audio ID");                        
            //            csvWriter.NextRecord();

            //            foreach (var track in finalTracks)
            //            {
            //                csvWriter.WriteField(track.Item2);
            //                csvWriter.NextRecord();
            //            }
            //            csvWriter.Flush();
            //        }
            //    }
            //}

            ////asset-id-to-sa-id.csv
            //using (var stream = new FileStream(@"D:\go-src\src\emvn-minions\youtube-asset-cli\input\STYE\asset-id-to-sa-id.csv", FileMode.Create))
            //{
            //    using (var writer = new StreamWriter(stream, Encoding.UTF8))
            //    {
            //        using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
            //        {
            //            csvWriter.Configuration.HasHeaderRecord = true;
            //            csvWriter.WriteField<string>("Asset ID");
            //            csvWriter.WriteField<string>("Source Audio ID");
            //            csvWriter.NextRecord();

            //            foreach (var track in finalTracks)
            //            {
            //                csvWriter.WriteField(track.Item1);
            //                csvWriter.WriteField(track.Item2);
            //                csvWriter.NextRecord();
            //            }
            //            csvWriter.Flush();
            //        }
            //    }
            //}

            var sravFile = @"D:\go-src\src\emvn-minions\youtube-asset-cli\input\STYE\SRAV Monitoring.csv";
            var reportFile = @"D:\go-src\src\emvn-minions\youtube-asset-cli\input\STYE\asset_full_report_Audiomachine_L_v1-2.csv";
            var finalMappingFile = @"D:\go-src\src\emvn-minions\youtube-asset-cli\input\STYE\STYE_EPIC ELITE ASSETS EXPORT WITH SA ID - STYE_EPIC ELITE ASSETS EXPORT ALL.csv";

            var sravStyeFile = @"D:\go-src\src\emvn-minions\youtube-asset-cli\input\STYE\SRAV Monitoring-STYE.csv";
            var sravStyenNoMappingFile = @"D:\go-src\src\emvn-minions\youtube-asset-cli\input\STYE\SRAV Monitoring-STYE-No Mapping.csv";
            var styeAssets = new Dictionary<string, YoutubeAsset>();
            var styeMissingArtTrackAssets = new List<string[]>();
            var styeMissingArtTrackNoMappingAssets = new List<string[]>();
            using (var streamReader = System.IO.File.OpenText(reportFile))
            {
                using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    reader.Read();
                    reader.ReadHeader();
                    while (reader.Read())
                    {
                        var assetType = reader.GetField("asset_type");
                        var assetID = reader.GetField("asset_id");
                        var label = reader.GetField("asset_label");
                        if (assetType.ToUpper() == "SOUND_RECORDING"
                            && label.ToUpper().Contains("SONGS TO YOUR EYES"))
                        {
                            styeAssets.Add(assetID, new YoutubeAsset()
                            {
                                AssetID = assetID
                            });
                        }
                    }
                }
            }

            var mappings = new Dictionary<string, string>();
            using (var streamReader = System.IO.File.OpenText(finalMappingFile))
            {
                using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    reader.Read();
                    reader.ReadHeader();
                    while (reader.Read())
                    {
                        var assetID = reader.GetField("Asset ID");
                        mappings.Add(assetID, assetID);
                    }
                }
            }

            var headerRecord = new string[0];
            using (var streamReader = System.IO.File.OpenText(sravFile))
            {
                using (var reader = new CsvHelper.CsvParser(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    headerRecord = reader.Read();
                    string[] record = null;
                    while ((record = reader.Read()) != null)
                    {                        
                        var assetID = record[2];
                        if (styeAssets.ContainsKey(assetID))
                        {
                            styeMissingArtTrackAssets.Add(record);
                            if (!mappings.ContainsKey(assetID))
                            {
                                styeMissingArtTrackNoMappingAssets.Add(record);
                            }
                        }
                    }
                }
            }            

            //output
            using (var stream = new FileStream(sravStyeFile, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
                    {
                        csvWriter.Configuration.HasHeaderRecord = true;
                        foreach (var header in headerRecord)
                        {
                            csvWriter.WriteField(header);
                        }
                        csvWriter.NextRecord();
                        foreach (var record in styeMissingArtTrackAssets)
                        {
                            foreach (var field in record)
                            {
                                csvWriter.WriteField(field);
                            }
                            csvWriter.NextRecord();
                        }
                        csvWriter.Flush();
                    }
                }
            }

            using (var stream = new FileStream(sravStyenNoMappingFile, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
                    {
                        csvWriter.Configuration.HasHeaderRecord = true;
                        foreach (var header in headerRecord)
                        {
                            csvWriter.WriteField(header);
                        }
                        csvWriter.NextRecord();
                        foreach (var record in styeMissingArtTrackNoMappingAssets)
                        {
                            foreach (var field in record)
                            {
                                csvWriter.WriteField(field);
                            }
                            csvWriter.NextRecord();
                        }
                        csvWriter.Flush();
                    }
                }
            }
        }
    }
}
