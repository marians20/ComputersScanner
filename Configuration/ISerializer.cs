namespace Configuration
{
    public interface ISerializer<T>
    {
        void SetFileName(string fileName);

        void Save(T data);

        T Load();
    }
}
