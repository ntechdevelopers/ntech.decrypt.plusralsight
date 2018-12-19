using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ntech.NetStandard.Utilities.DecryptPluralSight.Helper
{
    public static class PathExtension
    {
        public static string CleanPath(this string path, IList<char> invalidPathCharacters)
        {
            foreach (var invalidChar in invalidPathCharacters)
            {
                path = path.Replace(invalidChar, '-');
            }

            return path;
        }

        public static string EscapeIllegalCharacters(this string path)
        {
            // Windows NTFS (macOS is only ':')
            string illegalCharacters = "/?<>:*|\"";
            foreach (char c in illegalCharacters)
            {
                path = path.Replace(c.ToString(), String.Empty);
            }

            return path;
        }

        public static string RenameIfDuplicated(this string path)
        {
            string newFullPath = string.Empty;
            int count = 1;

            // If path is file
            if (Path.HasExtension(path))
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                string extension = Path.GetExtension(path);
                string currPath = Path.GetDirectoryName(path);
                newFullPath = path;

                while (File.Exists(newFullPath))
                {
                    string tempFileName = $"{fileName} ({count++})";
                    newFullPath = Path.Combine(currPath, tempFileName + extension);
                }
            }
            // Else path is directory
            else
            {
                string folderName = GetFolderName(path);
                string currPath = Path.GetDirectoryName(path);
                newFullPath = path;

                while (Directory.Exists(newFullPath))
                {
                    string tempFileName = $"{folderName} ({count++})";
                    newFullPath = Path.Combine(currPath, tempFileName);
                }
            }
            return newFullPath;
        }

        public static string GetFileNameIfItExisted(this string filePath, bool checkExisted = false)
        {
            if (checkExisted)
            {
                if (File.Exists(filePath))
                {
                    return filePath.Substring(filePath.LastIndexOf(@"\") + 1);
                }

                throw new FileNotFoundException();
            }
            return filePath.Substring(filePath.LastIndexOf(@"\") + 1);
        }

        public static string GetFolderName(this string folderPath, bool checkExisted = false)
        {
            if (checkExisted)
            {
                if (Directory.Exists(folderPath))
                {
                    return folderPath.Substring(folderPath.LastIndexOf(@"\") + 1);
                }
                throw new DirectoryNotFoundException();
            }
            return folderPath.Substring(folderPath.LastIndexOf(@"\") + 1);
        }

        public static bool CompareTwoFiles(this string fileOne, string fileTwo)
        {
            return File.ReadAllBytes(fileOne).LongLength == File.ReadAllBytes(fileTwo).LongLength;
        }
    }
}
