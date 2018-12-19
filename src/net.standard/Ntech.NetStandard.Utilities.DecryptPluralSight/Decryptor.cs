using Common.Logging;
using Ntech.NetStandard.Utilities.DecryptPluralSight.Encryption;
using Ntech.NetStandard.Utilities.DecryptPluralSight.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ntech.NetStandard.Utilities.DecryptPluralSight.Helper;
using Ntech.NetStandard.Utilities.DecryptPluralSight.Repository;

namespace Ntech.NetStandard.Utilities.DecryptPluralSight
{
    public class Decryptor
    {
        private static ILog Logger = LogManager.GetLogger(typeof(Decryptor));

        private VirtualFileStream playingFileStream;

        private IStream iStream;

        private List<char> InvalidPathCharacters = new List<char>();

        private List<char> InvalidFileCharacters = new List<char>();

        private List<Task> TaskList = new List<Task>();

        private SemaphoreSlim Semaphore = new SemaphoreSlim(5);

        private object SemaphoreLock = new object();

        private DatabaseSQLiteConnection databaseSQLiteConnection => new DatabaseSQLiteConnection(this.DatabasePath);

        public string DatabasePath { private get; set; }

        public Decryptor()
        {
            this.InvalidPathCharacters.AddRange(Path.GetInvalidPathChars());
            this.InvalidPathCharacters.AddRange(new char[] { ':', '?', '"', '\\', '/' });

            this.InvalidFileCharacters.AddRange(Path.GetInvalidFileNameChars());
            this.InvalidFileCharacters.AddRange(new char[] { ':', '?', '"', '\\', '/' });
        }

        public string ModuleHash(string moduleName, string moduleAuthorName)
        {
            var s = moduleName + "|" + moduleAuthorName;
            using (MD5 md5 = MD5.Create())
            {
                return Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(s))).Replace('/', '_');
            }
        }

        public void DecryptAllFolders(string folderPath, bool isCreateTranscript, string outputFolder = "", bool ignoreException = true)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException();
            }

            if (string.IsNullOrWhiteSpace(outputFolder) || !Directory.Exists(outputFolder))
            {
                outputFolder = folderPath;
            }

            foreach (string coursePath in Directory.GetDirectories(folderPath, "*", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    var courseRepository = databaseSQLiteConnection.Connect().GetCourseRepository();
                    var course = courseRepository.GetCourseFromDb(coursePath);
                    databaseSQLiteConnection.Disconnect();

                    if (course != null)
                    {
                        Logger.Info("Decryption " + course.CourseTitle);
                        var newCoursePath = Path.Combine(outputFolder, course.CourseTitle.CleanPath(this.InvalidPathCharacters));

                        if (!Directory.Exists(newCoursePath))
                        {
                            Directory.CreateDirectory(newCoursePath);
                        }

                        var courseInfo = new DirectoryInfo(newCoursePath);

                        var modulesRepository = databaseSQLiteConnection.Connect().GetModulesRepository();
                        var listModules = modulesRepository.GetModulesFromDb(course.CourseName);
                        databaseSQLiteConnection.Disconnect();

                        if (listModules.Count > 0)
                        {
                            foreach (Module module in listModules)
                            {
                                var moduleHash = this.ModuleHash(module.ModuleName, module.AuthorHandle);
                                var moduleHashPath = Path.Combine(coursePath, moduleHash);
                                var newModulePath = Path.Combine(courseInfo.FullName, module.ModuleIndex + ". " + module.ModuleTitle.CleanPath(this.InvalidPathCharacters));

                                // If length too long, rename it
                                if (newModulePath.Length > 240)
                                {
                                    newModulePath = Path.Combine(courseInfo.FullName, module.ModuleIndex + "");
                                }

                                //newModulePath = newModulePath.EscapeIllegalCharacters();

                                if (Directory.Exists(moduleHashPath))
                                {
                                    var moduleInfo = Directory.Exists(newModulePath) ? new DirectoryInfo(newModulePath) : Directory.CreateDirectory(newModulePath);
                                    this.DecryptAllVideos(moduleHashPath, module.ModuleId, newModulePath, isCreateTranscript);
                                }
                                else
                                {
                                    Logger.Error("Folder " + moduleHash + " cannot be found in the current course path.");
                                    if (!ignoreException)
                                    {
                                        throw new Exception("Folder " + moduleHash + " cannot be found in the current course path.");
                                    }

                                }
                            }
                        }
                        Logger.Info("Decryption " + course.CourseTitle + " has been completed!");
                    }
                }
                catch (Exception ex)
                {
                    if (!ignoreException)
                    {
                        throw new Exception($"Cannot be found in the current course path. {ex}");
                    }
                }
            }
        }

        public void DecryptAllVideos(string folderPath, int moduleId, string outputPath, bool isCreateTranscript, bool ignoreException = true)
        {
            var clipsRepository = databaseSQLiteConnection.Connect().GetClipsRepository();
            var listClips = clipsRepository.GetClipsFromDb(moduleId);
            databaseSQLiteConnection.Disconnect();

            if (listClips.Count > 0)
            {
                foreach (Clip clip in listClips)
                {
                    try
                    {
                        var currPath = Path.Combine(folderPath, clip.ClipName + ".psv");
                        if (File.Exists(currPath))
                        {
                            var newPath = Path.Combine(outputPath, clip.ClipIndex + ". " + clip.ClipTitle.CleanPath(this.InvalidPathCharacters) + ".mp4");

                            // If length too long, rename it
                            if (newPath.Length > 240)
                            {
                                newPath = Path.Combine(outputPath, clip.ClipIndex + ".mp4");
                            }

                            //newPath = newPath.EscapeIllegalCharacters();

                            playingFileStream = new VirtualFileStream(currPath);
                            playingFileStream.Clone(out iStream);

                            var fileName = Path.GetFileName(currPath);
                            Logger.Info($"Start to Decrypt File {fileName}");

                            Semaphore.Wait();
                            TaskList.Add(Task.Run(() =>
                            {
                                DecryptVideo(iStream, newPath);
                                if (isCreateTranscript)
                                {
                                    WriteTranscriptFile(clip.ClipId, newPath);
                                }
                                lock (SemaphoreLock)
                                {
                                    Semaphore.Release();
                                }
                            }));

                            Logger.Info($"Decryption File { Path.GetFileName(newPath) } successfully");
                        }
                        else
                        {
                            if (!ignoreException)
                            {
                                throw new Exception($"File { Path.GetFileName(currPath) } cannot be found.");
                            }
                            Logger.ErrorFormat($"File {Path.GetFileName(currPath)} cannot be found.");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!ignoreException)
                        {
                            throw new Exception($"Cannot decrypt clip. {ex}");
                        }
                    }
                }
            }
        }

        public void WriteTranscriptFile(int clipId, string clipPath)
        {

            var trasncriptRepository = databaseSQLiteConnection.Connect().GetTrasncriptRepository();
            var clipTranscripts = trasncriptRepository.GetTrasncriptFromDb(clipId);
            databaseSQLiteConnection.Disconnect();

            if (clipTranscripts.Count > 0)
            {
                var transcriptPath = Path.Combine(Path.GetDirectoryName(clipPath), Path.GetFileNameWithoutExtension(clipPath) + ".srt");
                if (!File.Exists(transcriptPath))
                {
                    Logger.Info($"Transcript of " + Path.GetFileName(clipPath));
                    StreamWriter writer = new StreamWriter(transcriptPath);
                    int i = 1;
                    foreach (var clipTranscript in clipTranscripts)
                    {
                        var start = TimeSpan.FromMilliseconds(clipTranscript.StartTime).ToString(@"hh\:mm\:ss\,fff");
                        var end = TimeSpan.FromMilliseconds(clipTranscript.EndTime).ToString(@"hh\:mm\:ss\,fff");
                        writer.WriteLine(i++);
                        writer.WriteLine(start + " --> " + end);
                        writer.WriteLine(clipTranscript.Text);
                        writer.WriteLine();
                    }
                    writer.Close();
                    Logger.Info($"Transcript of " + Path.GetFileName(clipPath) + "has been generated scucessfully.");
                }
            }
        }

        public void DecryptVideo(IStream curStream, string newPath)
        {
            STATSTG stat;
            curStream.Stat(out stat, 0);
            IntPtr myPtr = (IntPtr)0;
            int strmSize = (int)stat.cbSize;
            byte[] strmInfo = new byte[strmSize];
            curStream.Read(strmInfo, strmSize, myPtr);
            File.WriteAllBytes(newPath, strmInfo);
        }

        public void RemoveCourse(string coursePath)
        {
            var courseRepository = databaseSQLiteConnection.Connect().GetCourseRepository();
            courseRepository.RemoveCourseInDb(coursePath);
            databaseSQLiteConnection.Disconnect();
        }
    }
}
