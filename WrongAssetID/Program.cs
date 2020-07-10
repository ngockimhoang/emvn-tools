using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongAssetID
{
    class Program
    {
        static void Main(string[] args)
        {
            var ddexPath = @"D:\GoProjects\src\emvn-minions\youtube-asset-cli\output\ddex_packages";
            var referencePath = @"C:\Users\kimhoang\Desktop\EMVN\take_down_reference.csv";
            var references = new List<string>();

            foreach (var directory in Directory.GetDirectories(ddexPath))
            {
                var directoryName = Path.GetFileName(directory);
                if (directoryName.StartsWith("SPL0041")
                    || directoryName.StartsWith("SPL0042")
                    || directoryName.StartsWith("SPL0043")
                    || directoryName.StartsWith("SPL0044")
                    || directoryName.StartsWith("SPL0045")
                    || directoryName.StartsWith("SPL0046")
                    || directoryName.StartsWith("SPL0047")
                    || directoryName.StartsWith("SPL0048")
                    || directoryName.StartsWith("SPL0049")
                    || directoryName.StartsWith("SPL0050"))
                {
                    var ackFilePath = Directory.GetFiles(directory, "ACK_*")[0];
                    var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(AckMessage));
                    var ackMessage = xmlSerializer.Deserialize(File.OpenText(ackFilePath)) as AckMessage;
                    var wrongReferences = ackMessage.AffectedResources.Where(p => string.IsNullOrEmpty(ackMessage.ErrorText) || !ackMessage.ErrorText.Contains(p.Reference)).Select(p => p.Reference).ToArray();
                    references.AddRange(wrongReferences);
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

                        foreach (var reference in references)
                        {
                            csvWriter.WriteField(reference);
                            csvWriter.WriteField("deactivate");
                            csvWriter.WriteField("");
                            csvWriter.NextRecord();
                        }
                        csvWriter.Flush();
                    }
                }
            }
        }
    }
}
