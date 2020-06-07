using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Common;

namespace CheckAlbumValid
{
    class Program
    {
        static void Main(string[] args)
        {
            var albumFoldersPath = @"D:\GoProjects\src\emvn-minions\youtube-asset-cli\output\album_tracks";
            
            var assetList = new List<CmsAsset>();
            var duplicateAssetList = new List<CmsAsset>();

            foreach (var file in Directory.GetFiles(albumFoldersPath, "*.json", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(file);
                if (fileName.StartsWith("APL"))
                {
                    var cmsAlbum = JsonConvert.DeserializeObject<CmsAlbum>(File.ReadAllText(file));                                        
                    //check duplicate
                    var dupAssets = cmsAlbum.Assets.GroupBy(p => p.ISRC).Where(p => p.Count() > 1).SelectMany(p => p.ToArray());
                    if (dupAssets.Any())
                        duplicateAssetList.AddRange(dupAssets);
                }

                
            }

            //using (var stream = new FileStream(outputPath, FileMode.OpenOrCreate))
            //{
            //    using (var writer = new StreamWriter(stream, Encoding.UTF8))
            //    {
            //        using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
            //        {
            //            csvWriter.Configuration.HasHeaderRecord = true;
            //            csvWriter.WriteField<string>("Album_Code");
            //            csvWriter.WriteField<string>("Album_Title");
            //            csvWriter.WriteField<string>("Asset_ID");
            //            csvWriter.WriteField<string>("ISRC");
            //            csvWriter.WriteField<string>("Song_Title");
            //            csvWriter.WriteField<string>("Filename");
            //            csvWriter.NextRecord();

            //            foreach (var cmsAsset in assetList)
            //            {
            //                csvWriter.WriteField(cmsAsset.AlbumCode);
            //                csvWriter.WriteField(cmsAsset.AlbumTitle);
            //                csvWriter.WriteField(cmsAsset.AssetID);
            //                csvWriter.WriteField(cmsAsset.ISRC);
            //                csvWriter.WriteField(cmsAsset.SongTitle);
            //                csvWriter.WriteField(cmsAsset.Filename);
            //                csvWriter.NextRecord();
            //            }

            //            csvWriter.Flush();
            //        }
            //    }
            //}

            //using (var stream = new FileStream(outputDuplicatePath, FileMode.OpenOrCreate))
            //{
            //    using (var writer = new StreamWriter(stream, Encoding.UTF8))
            //    {
            //        using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
            //        {
            //            csvWriter.Configuration.HasHeaderRecord = true;
            //            csvWriter.WriteField<string>("Album_Code");
            //            csvWriter.WriteField<string>("Album_Title");
            //            csvWriter.WriteField<string>("Asset_ID");
            //            csvWriter.WriteField<string>("ISRC");
            //            csvWriter.WriteField<string>("Song_Title");
            //            csvWriter.WriteField<string>("Filename");
            //            csvWriter.NextRecord();

            //            foreach (var cmsAsset in duplicateAssetList)
            //            {
            //                csvWriter.WriteField(cmsAsset.AlbumCode);
            //                csvWriter.WriteField(cmsAsset.AlbumTitle);
            //                csvWriter.WriteField(cmsAsset.AssetID);
            //                csvWriter.WriteField(cmsAsset.ISRC);
            //                csvWriter.WriteField(cmsAsset.SongTitle);
            //                csvWriter.WriteField(cmsAsset.Filename);
            //                csvWriter.NextRecord();
            //            }

            //            csvWriter.Flush();
            //        }
            //    }
            //}
        }
    }
}
