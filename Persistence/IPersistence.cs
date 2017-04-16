namespace Persistence
{
    public interface IPersistence
    {
        bool Save(string key, string value);
    }
}
