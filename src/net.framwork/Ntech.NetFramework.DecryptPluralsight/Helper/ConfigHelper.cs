namespace Ntech.NetFramework.DecryptPluralsight.Helper
{
    using System;
    using System.IO;
    using System.Xml.Serialization;
    using log4net;

    public static class ConfigHelper
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ConfigHelper));

        private const string DefaultSettingFile = "Setting.xml";

        private static string settingFilePath => Path.Combine(Environment.CurrentDirectory, DefaultSettingFile);

        private static void Serialize<T>(T source, string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var fileWriter = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(fileWriter, source);
            }
        }

        private static T Deserialize<T>(string filePath)
        {
            using (var fileWriter = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T) serializer.Deserialize(fileWriter);
            }
        }
    }
}
