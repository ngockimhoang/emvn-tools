using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckSAUserTrackStatReport
{
    class Program
    {
        static void Main(string[] args)
        {
            var saReport = @"D:\emvn-data\users-tracks-stats-2021-12-01-to-2021-12-31.csv";
            var output = @"D:\emvn-data\users-tracks-stats-2021-12-01-to-2021-12-31-duplicate.csv";
            var userTrackStatMap = new Dictionary<UserTrackStat, int>();
            var recordCount = 0;
            using (var streamReader = System.IO.File.OpenText(saReport))
            {
                using (var reader = new CsvHelper.CsvReader(streamReader, System.Threading.Thread.CurrentThread.CurrentCulture))
                {
                    reader.Read();
                    reader.ReadHeader();
                    while (reader.Read())
                    {
                        recordCount++;
                        var userTrackStat = new UserTrackStat()
                        {
                            UserID = reader.GetField("User ID"),
                            Email = reader.GetField("Email"),
                            SourceAudioID = reader.GetField<int>("SourceAudio ID"),
                            Time = reader.GetField("Time"),
                            InteractionType = reader.GetField("Interaction Type")
                        };                        
                        if (userTrackStatMap.ContainsKey(userTrackStat))
                        {
                            userTrackStatMap[userTrackStat] += 1;
                        }
                        else
                        {
                            userTrackStatMap[userTrackStat] = 1;
                        }
                    }
                }
            }

            var duplicateCount = userTrackStatMap.Where(p => p.Value > 1).Count();
            Console.WriteLine("Original Record Count: " + recordCount.ToString());
            Console.WriteLine("Duplicate count: " + duplicateCount.ToString());
            Console.WriteLine("Unique Record count: " + userTrackStatMap.Count().ToString());

            using (var outputStream = new FileStream(output, FileMode.Create))
            {
                using (var writer = new StreamWriter(outputStream, Encoding.UTF8))
                {
                    using (var csvWriter = new CsvHelper.CsvWriter(writer, System.Threading.Thread.CurrentThread.CurrentCulture))
                    {
                        csvWriter.WriteField("User ID");
                        csvWriter.WriteField("Email");
                        csvWriter.WriteField("SourceAudio ID");
                        csvWriter.WriteField("Time");
                        csvWriter.WriteField("Interaction Type");
                        csvWriter.WriteField("Record Count");                                                
                        csvWriter.NextRecord();

                        foreach (var duplicateUserTrackStat in userTrackStatMap.Where(p => p.Value > 1))
                        {
                            csvWriter.WriteField(duplicateUserTrackStat.Key.UserID);
                            csvWriter.WriteField(duplicateUserTrackStat.Key.Email);
                            csvWriter.WriteField(duplicateUserTrackStat.Key.SourceAudioID);
                            csvWriter.WriteField(duplicateUserTrackStat.Key.Time);
                            csvWriter.WriteField(duplicateUserTrackStat.Key.InteractionType);
                            csvWriter.WriteField(duplicateUserTrackStat.Value);
                            csvWriter.NextRecord();

                            csvWriter.Flush();
                        }                        
                    }
                }
            }

            Console.ReadKey();
        }
    }

    class UserTrackStat
    {
        public string UserID { get; set; }
        public string Email { get; set; }
        public int SourceAudioID { get; set; }
        public string Time { get; set; }
        public string InteractionType { get; set; }

        public override int GetHashCode()
        {
            return string.Format("{0}_{1}_{2}_{3}", UserID, SourceAudioID, Time, InteractionType).GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as UserTrackStat);
        }

        public bool Equals(UserTrackStat obj)
        {
            return obj != null 
                && obj.UserID == this.UserID
                && obj.SourceAudioID == this.SourceAudioID
                && obj.Time == this.Time
                && obj.InteractionType == this.InteractionType;
        }
    }
}
