namespace Configuration
{
    public abstract class GenericSerializer<T> : ISerializer<T>
    {
        public string FileName { get; set; }

        public void SetFileName(string fileName)
        {
            FileName = fileName;
        }

        public abstract T Load();

        public abstract void Save(T data);
    }
}
