namespace Play.Catalog.Service.Settings
{
    public class MongoDbSettings
    {
        public string Host { get; init; } // init allows us to not change the settings after the service loads, just initialize it 
        public int Port { get; init; }
        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }
}