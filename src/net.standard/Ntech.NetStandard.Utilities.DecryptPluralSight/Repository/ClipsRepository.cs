using Ntech.NetStandard.Utilities.DecryptPluralSight.Model;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Ntech.NetStandard.Utilities.DecryptPluralSight.Repository
{
    public class ClipsRepository
    {
        private SQLiteConnection databaseConnection;

        public ClipsRepository(SQLiteConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public List<Clip> GetClipsFromDb(int moduleId)
        {
            List<Clip> list = new List<Clip>();

            var cmd = databaseConnection.CreateCommand();
            cmd.CommandText = @"SELECT Id, Name, Title, ClipIndex
                                FROM Clip 
                                WHERE ModuleId = @moduleId";
            cmd.Parameters.Add(new SQLiteParameter("@moduleId", moduleId));

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Clip clip = new Clip
                {
                    ClipId = reader.GetInt32(reader.GetOrdinal("Id")),
                    ClipName = reader.GetString(reader.GetOrdinal("Name")),
                    ClipTitle = reader.GetString(reader.GetOrdinal("Title")),
                    ClipIndex = reader.GetInt32(reader.GetOrdinal("ClipIndex"))
                };
                list.Add(clip);
            }
            reader.Close();
            return list;
        }
    }
}
