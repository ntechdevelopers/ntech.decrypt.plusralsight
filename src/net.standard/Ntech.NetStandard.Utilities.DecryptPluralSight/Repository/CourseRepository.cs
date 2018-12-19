using Ntech.NetStandard.Utilities.DecryptPluralSight.Helper;
using Ntech.NetStandard.Utilities.DecryptPluralSight.Model;
using System.Data.SQLite;

namespace Ntech.NetStandard.Utilities.DecryptPluralSight.Repository
{
    public class CourseRepository
    {
        private SQLiteConnection databaseConnection;

        public CourseRepository(SQLiteConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public bool RemoveCourseInDb(string coursePath)
        {
            string courseName = coursePath.GetFolderName();

            var cmd = databaseConnection.CreateCommand();
            cmd.CommandText = @"DELETE FROM Course WHERE Name = @courseName";
            cmd.Parameters.Add(new SQLiteParameter("@courseName", courseName));

            var reader = cmd.ExecuteNonQuery();

            return reader > 0;
        }

        public Course GetCourseFromDb(string folderCoursePath)
        {
            Course course = null;

            string courseName = folderCoursePath.GetFolderName(true).Trim().ToLower();

            var cmd = databaseConnection.CreateCommand();
            cmd.CommandText = @"SELECT Name, Title, HasTranscript 
                                FROM Course 
                                WHERE Name = @courseName";
            cmd.Parameters.Add(new SQLiteParameter("@courseName", courseName));

            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                course = new Course
                {
                    CourseName = reader.GetString(reader.GetOrdinal("Name")),
                    CourseTitle = reader.GetString(reader.GetOrdinal("Title")),
                    HasTranscript = reader.GetInt32(reader.GetOrdinal("HasTranscript"))
                };
            }

            reader.Close();

            return course;
        }
    }
}
