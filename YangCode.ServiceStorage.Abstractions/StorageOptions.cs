namespace YangCode.ServiceStorage.Abstractions
{
    public class StorageOptions
    {
        private IStorageOptionsExtension? _extension;
        private string? _name;
        public IStorageOptionsExtension? Extension => _extension;
        public string? Prefix => _name;

        public StorageOptions WithPrefix(string prefix)
        {
            _name = prefix;
            return this;
        }

        public StorageOptions WithExtension(IStorageOptionsExtension extension)
        {
            _extension = extension;
            return this;
        }
    }
}