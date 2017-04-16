using Newtonsoft.Json;
using System.IO;

namespace Configuration
{
    public class JsonSerializer<T>: GenericSerializer<T>
    {
        public override T Load()
        {
            return File.Exists(FileName)
                ? JsonConvert.DeserializeObject<T>(File.ReadAllText(FileName))
                : default(T);
        }

        public override void Save(T data)
        {
            File.WriteAllText(FileName, JsonConvert.SerializeObject(data, Formatting.Indented));
        }
    }
}
