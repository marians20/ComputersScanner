namespace Configuration
{
    public enum Serializers { None, Yaml, Json };
    public class SerializerFactory<T>
    {
        public static ISerializer<T> GetSerializer(Serializers serializerKind)
        {
            ISerializer<T> result;
            switch(serializerKind)
            {
                case Serializers.Json:
                    result = new JsonSerializer<T>();
                    break;
                case Serializers.Yaml:
                    result = new YamlSerializer<T>();
                    break;
                default:
                    result = default(ISerializer<T>);
                    break;
            }
            return result;
        }
    }
}
