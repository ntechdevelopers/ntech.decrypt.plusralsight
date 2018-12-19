using Ntech.NetStandard.Utilities.DecryptPluralSight.Model;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Ntech.NetStandard.Utilities.DecryptPluralSight.Repository
{
    public class ModulesRepository
    {
        private SQLiteConnection databaseConnection;

        public ModulesRepository(SQLiteConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public List<Module> GetModulesFromDb(string courseName)
        {
            List<Module> list = new List<Module>();

            var cmd = databaseConnection.CreateCommand();
            cmd.CommandText = @"SELECT Id, Name, Title, AuthorHandle, ModuleIndex
                                FROM Module 
                                WHERE CourseName = @courseName";
            cmd.Parameters.Add(new SQLiteParameter("@courseName", courseName));

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Module module = new Module
                {
                    ModuleId = reader.GetInt32(reader.GetOrdinal("Id")),
                    AuthorHandle = reader.GetString(reader.GetOrdinal("AuthorHandle")),
                    ModuleName = reader.GetString(reader.GetOrdinal("Name")),
                    ModuleTitle = reader.GetString(reader.GetOrdinal("Title")),
                    ModuleIndex = reader.GetInt32(reader.GetOrdinal("ModuleIndex"))
                };
                list.Add(module);
            }
            reader.Close();
            return list;
        }
    }
}
