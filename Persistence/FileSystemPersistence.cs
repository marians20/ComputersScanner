using System.IO;
using Logger;

namespace Persistence
{
    public class FileSystemPersistence : IPersistence
    {
        public string BasePath { get; set; }

        public FileSystemPersistence(string basePath)
        {
            BasePath = basePath;
        }
        public bool Save(string key, string value)
        {
            object locker = new object();
            lock (locker)
            {
                var fileName = string.Format("{0}.{1}", key, "json");
                var filePath = Path.Combine(BasePath, fileName);
                Log.Debug("Saving file {0}", filePath);
                File.WriteAllText(filePath, value);
            }
            return true;
        }
    }
}
