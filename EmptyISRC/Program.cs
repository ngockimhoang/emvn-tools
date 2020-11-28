using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmptyISRC
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"D:\go-src\src\emvn-minions\youtube-asset-cli\input\source-audio-tracks.csv";
            var outputFolder = @"D:\emvn-data\empty-isrc";
            var header = new string[0];
            //dictionary by catalog
            var assetDict = new Dictionary<string, List<string[]>>();
            using (var reader = File.OpenText(path))
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
                        var catalog = record[3].ToUpper();
                        var albumCode = record[6];
                        var isrc = record[33];
                        if (string.IsNullOrWhiteSpace(albumCode))
                            continue;
                        if (string.IsNullOrWhiteSpace(isrc))
                        {
                            var assetList = new List<string[]>();
                            if (assetDict.ContainsKey(catalog))
                            {
                                assetList = assetDict[catalog];
                            }
                            else assetDict.Add(catalog, assetList);
                            assetList.Add(record);
                        }                                               
                    }
                }
            }

            //SOUND IDEAS (SFX LIBRARY)
            //HOLLYWOOD EDGE (SFX LIBRARY)
            //SOUNDDOGS

            var excludes = new string[] { "SOUND IDEAS (SFX LIBRARY)", "HOLLYWOOD EDGE (SFX LIBRARY)", "SOUNDDOGS" };
            var total = assetDict.Where(p => !excludes.Contains(p.Key)).Sum(p => p.Value.Count);

            //write to disk
            foreach (var catalog in assetDict)
            {
                if (excludes.Contains(catalog.Key))
                    continue;

                using (var writer = File.CreateText(Path.Combine(outputFolder, catalog.Key + ".csv")))
                {
                    using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
                    {
                        foreach (var headerName in header)
                        {
                            csvWriter.WriteField(headerName);
                        }
                        csvWriter.NextRecord();

                        foreach (var asset in catalog.Value)
                        {
                            foreach (var field in asset)
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
