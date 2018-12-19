using Ntech.NetStandard.Utilities.DecryptPluralSight.Model;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Ntech.NetStandard.Utilities.DecryptPluralSight.Repository
{
    public class TrasncriptRepository
    {
        private SQLiteConnection databaseConnection;

        public TrasncriptRepository(SQLiteConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public List<ClipTranscript> GetTrasncriptFromDb(int clipId)
        {
            var list = new List<ClipTranscript>();

            var cmd = databaseConnection.CreateCommand();
            cmd.CommandText = @"SELECT StartTime, EndTime, Text
                                FROM ClipTranscript
                                WHERE ClipId = @clipId
                                ORDER BY Id ASC";
            cmd.Parameters.Add(new SQLiteParameter("@clipId", clipId));

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ClipTranscript clipTranscript = new ClipTranscript
                {
                    StartTime = reader.GetInt32(reader.GetOrdinal("StartTime")),
                    EndTime = reader.GetInt32(reader.GetOrdinal("EndTime")),
                    Text = reader.GetString(reader.GetOrdinal("Text"))
                };
                list.Add(clipTranscript);
            }

            return list;
        }
    }
}
