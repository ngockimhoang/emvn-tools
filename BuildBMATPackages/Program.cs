using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildBMATPackages
{
    class Program
    {
        static void Main(string[] args)
        {
            var catalogs = new Catalog[] 
            {
                new Catalog() 
                { 
                    Name = "APL Publishing",
                    Labels = new string[] { "APL", "APL Decades", "APL Frontrunners", "APL Lifestyle", "APL Organic", "APL Vocals", "Swimming Pool" }
                },
                new Catalog()
                {
                    Name = "Audio Attack",
                    Labels = new string[] { "Audio Attack Trailer Series", "Epic Single Series" }
                },
                new Catalog()
                {
                    Name = "Audiomachine",
                    Labels = new string[] { "Cover Songs", "Studio Series", "Trailer Music" }
                },
                new Catalog()
                {
                    Name = "Brand X Production Music",
                    Labels = new string[] { "Production Music" }
                },
                new Catalog()
                {
                    Name = "Brand X Trailer Music",
                    Labels = new string[] { "Trailer Music" }
                },
                new Catalog()
                {
                    Name = "Catapult Music",
                    Labels = new string[] { "Catapult Music" }
                },
                new Catalog()
                {
                    Name = "Cue Source Music",
                    Labels = new string[] { "Cue Source Music (CS)" }
                },
            };

            var cliFolder = @"D:\go-src\src\emvn-minions\youtube-asset-cli";
            foreach (var catalog in catalogs)
            {
                foreach (var label in catalog.Labels)
                {
                    var startInfo = new ProcessStartInfo("cmd");
                    startInfo.WorkingDirectory = cliFolder;
                    startInfo.Arguments = string.Format("/K go run \"{0}/main.go\" --catalog \"{1}\" generate-grid", cliFolder, catalog.Name, label);
                    startInfo.WindowStyle = ProcessWindowStyle.Normal;
                    var process = Process.Start(startInfo);
                    process.WaitForExit();
                }
            }
            

            
        }
    }

    internal class Catalog
    {
        internal string Name { get; set; }
        internal string[] Labels { get; set; }
    }
}
