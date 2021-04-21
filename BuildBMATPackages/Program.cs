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
                new Catalog()
                {
                    Name = "EMVN",
                    Labels = new string[] { "Epic Music VN" }
                },
                new Catalog()
                {
                    Name = "Epicenter Music",
                    Labels = new string[] { "Epicenter Music" }
                },
                new Catalog()
                {
                    Name = "Flippermusic",
                    Labels = new string[] { "Primrose Music (PRM)" }
                },
                new Catalog()
                {
                    Name = "Gothic Storm Music",
                    Labels = new string[] { "Gothic Hybrid", "Gothic Storm" }
                },
                new Catalog()
                {
                    Name = "HollywoodTrax",
                    Labels = new string[] { "HollywoodTrax" }
                },
                new Catalog()
                {
                    Name = "Melosy",
                    Labels = new string[] { "Instant Audio", "vMelody", "vTunes" }
                },
                new Catalog()
                {
                    Name = "Music For Film",
                    Labels = new string[] { "Future Pop", "Library Of The Human Soul", "Lovely Music Library", "Minim", "Songcraft" }
                },
                new Catalog()
                {
                    Name = "Pink Shark Music",
                    Labels = new string[] { "PSMGS","PSM Songs","PSMNX","PSMX Songs","PSMX" }
                },
                new Catalog()
                {
                    Name = "Pitch Hammer Music",
                    Labels = new string[] { "Additional Alt Mixes", "Pitch Hammer Full Mixes" }
                },
                new Catalog()
                {
                    Name = "Really Slow Motion Music",
                    Labels = new string[] { "Really Slow Motion Music" }
                },
                new Catalog()
                {
                    Name = "Spirit Production Music",
                    Labels = new string[] 
                    { 
                        "Artist Series",
                        "Build Destroy Music",
                        "Counter Communications",
                        "Evocativity",
                        "Fantastic Beat Machine Records",
                        "Fire Sticks Gaming Productions",
                        "George Shaw Music Library",
                        "IStandard",
                        "Jammin' Planet",
                        "Jim Bacchi Catalog",
                        "Me Enemy Music",
                        "Mucho Fruto Music",
                        "New World Music",
                        "Score Suite",
                        "Two Crowns Publishing",
                        "Musical Concepts",
                        "Opus 1 Music Library",
                        "Paradise Music",
                        "Pure",
                        "Pure Music",
                        "Soul Studio 7",
                        "Vintage C's" 
                    }
                },
                new Catalog()
                {
                    Name = "Songs To Your Eyes",
                    Labels = new string[] { "Songs To Your Eyes Music" }
                },
                new Catalog()
                {
                    Name = "Sonoton",
                    Labels = new string[] 
                    {
                        "Afro Dizzy - ADZ",
                        "Commercial Length Cuts - CLC",
                        "Commercials Non Stop - CNS",
                        "Intersound - ISCD",
                        "Intersound - SPOTS",
                        "Pro Viva - ISPV",
                        "Rockshop - ROCK",
                        "Sonia Classics - SONIA"
                    }
                },
                new Catalog()
                {
                    Name = "SPM Music Group",
                    Labels = new string[]
                    {
                        "Colossal Trailer Music",
                        "Hollywood Buzz Music",
                        "Rave Train Music"
                    }
                },
                 new Catalog()
                {
                    Name = "Westar Music",
                    Labels = new string[]
                    {
                        "American Classics",
                        "Blues",
                        "Children",
                        "Classical",
                        "Comedy & Cartoon",
                        "Corporate",
                        "Country",
                        "Dance & Club",
                        "Drama & Film Score",
                        "Drama & Film Scores",
                        "Easy Listening",
                        "Extreme Sports",
                        "Holidays & Christmas",
                        "Jazz",
                        "Latin Music",
                        "Military & Marches",
                        "New Age",
                        "New Country",
                        "News",
                        "Old School",
                        "Percussion",
                        "Period Dance",
                        "Period Jazz",
                        "Retro TV",
                        "Rock",
                        "Romance",
                        "Solo Instrument",
                        "Sports",
                        "Urban Music",
                        "Western",
                        "World Fusion",
                        "World Music"
                    }
                },
            };

            var cliFolder = @"D:\go-src\src\emvn-minions\youtube-asset-cli";
            foreach (var catalog in catalogs)
            {
                foreach (var label in catalog.Labels)
                {
                    var startInfo = new ProcessStartInfo("cmd");
                    startInfo.WorkingDirectory = cliFolder;
                    startInfo.Arguments = string.Format("/K go run \"{0}/main.go\" --catalog \"{1}\" --label \"{2}\" build-bmat-package", cliFolder, catalog.Name, label);
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
