using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace Ntech.NetStandard.Utilities.DecryptPluralSight.Repository
{
    public class DatabaseSQLiteConnection
    {
        private static ILog Logger = LogManager.GetLogger(typeof(DatabaseSQLiteConnection));

        private SQLiteConnection databaseConnection;

        private CourseRepository courseRepository;

        private ClipsRepository clipsRepository;

        private ModulesRepository modulesRepository;

        private TrasncriptRepository trasncriptRepository;

        private string dbPath;

        public DatabaseSQLiteConnection(string dbPath)
        {
            this.dbPath = dbPath;
        }

        public DatabaseSQLiteConnection Connect()
        {
            if (File.Exists(dbPath))
            {
                if (Path.GetExtension(dbPath).Equals(".db"))
                {
                    databaseConnection = new SQLiteConnection($"Data Source={dbPath}; Version=3;FailIfMissing=True");
                    databaseConnection.Open();
                    Logger.Info("The Database Connection has been open completely.");

                    return this;
                }
                else
                {
                    throw new Exception("The database file isn't corrected.");
                }
            }
            else
            {
                throw new Exception("Cannot find the database path.");
            }
        }

        public DatabaseSQLiteConnection Disconnect()
        {
            databaseConnection?.Close();
            return this;
        }

        public CourseRepository GetCourseRepository()
        {
            this.courseRepository = new CourseRepository(databaseConnection);
            return this.courseRepository;
        }

        public ClipsRepository GetClipsRepository()
        {
            this.clipsRepository = new ClipsRepository(databaseConnection);
            return this.clipsRepository;
        }

        public ModulesRepository GetModulesRepository()
        {
            this.modulesRepository = new ModulesRepository(databaseConnection);
            return this.modulesRepository;
        }

        public TrasncriptRepository GetTrasncriptRepository()
        {
            this.trasncriptRepository = new TrasncriptRepository(databaseConnection);
            return this.trasncriptRepository;
        }
    }
}
