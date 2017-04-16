using System.IO;

namespace Configuration
{
    public class YamlSerializer<T> : GenericSerializer<T>
    {
        public override T Load()
        {
            var parser = new YamlDotNet.Serialization.Deserializer();

            using (var input = new StreamReader(FileName))
            {
                var config = parser.Deserialize<T>(input);
                return config;
            }
        }

        public override void Save(T data)
        {
            var serializer = new YamlDotNet.Serialization.Serializer();
            using (TextWriter writer = File.CreateText(FileName))
            {
                serializer.Serialize(writer, data);
            }
        }
    }
}
