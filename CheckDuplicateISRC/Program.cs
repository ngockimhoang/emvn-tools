using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CheckDuplicateISRC
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceAudioTracksPath = @"C:\Users\kimhoang\Desktop\EMVN\source-audio-tracks-apl.csv";
            var sourceAudioTracksISRCPath = @"D:\GoProjects\src\emvn-minions\youtube-asset-cli\input\tracks_isrc.csv";
            var outputPath = @"C:\Users\kimhoang\Desktop\EMVN\source-audio-tracks-apl-duplicate-isrc.csv";
            var assetDict = new Dictionary<string, List<string[]>>();
            var sourceAudioISRCDict = new Dictionary<string, string>();
            var header = new string[0];
            using (var reader = File.OpenText(sourceAudioTracksISRCPath))
            {
                using (var csvReader = new CsvHelper.CsvParser(reader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    csvReader.Read();
                    while (true)
                    {
                        var record = csvReader.Read();
                        if (record == null
                            || !record.Any())
                            break;
                        var isrc = record[0].Trim().ToLower();
                        var sourceAudioID = record[1].Trim();
                        if (isrc != "delete")
                        {
                            sourceAudioISRCDict.Add(sourceAudioID, isrc.ToUpper());
                        }
                    }
                }
            }
            using (var reader = File.OpenText(sourceAudioTracksPath))
            {
                using (var csvReader = new CsvHelper.CsvParser(reader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    header = csvReader.Read();
                    while (true)
                    {
                        var record = csvReader.Read();
                        if (record == null
                            || !record.Any())
                            break;
                        var isrc = record[33];
                        if (string.IsNullOrWhiteSpace(isrc))
                        {
                            var sourceAudioID = record[0];
                            if (sourceAudioISRCDict.ContainsKey(sourceAudioID))
                            {
                                isrc = sourceAudioISRCDict[sourceAudioID];
                                record[33] = isrc;
                            }
                        }
                        if (string.IsNullOrWhiteSpace(isrc))
                            continue;
                        var assetList = new List<string[]>();
                        if (assetDict.ContainsKey(isrc))
                        {
                            assetList = assetDict[isrc];
                        }
                        else assetDict.Add(isrc, assetList);
                        assetList.Add(record);
                    }                              
                }
            }

            var duplicateList = new List<string[]>();
            foreach (var assetList in assetDict)
            {
                if (assetList.Value.Count > 1)
                {
                    duplicateList.AddRange(assetList.Value);
                }
            }

            using (var writer = File.CreateText(outputPath))
            {
                using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    csvWriter.WriteField("ISRC");
                    foreach (var headerName in header)
                    {
                        csvWriter.WriteField(headerName);
                    }
                    csvWriter.NextRecord();

                    foreach (var duplicateAsset in duplicateList)
                    {
                        csvWriter.WriteField(duplicateAsset[33]);
                        foreach (var field in duplicateAsset)
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
