using StackExchange.Redis;
using Logger;

namespace Persistence
{
    public class RedisPersistence : IPersistence
    {
        private string Server {get; set;}
        private int Port { get; set; }
        private string Password { get; set; }

        private readonly IDatabase _db;

        public RedisPersistence(string server, int port=5200, string password = "")
        {
            Server = server;
            Port = port;
            Password = password;
            var redis = ConnectionMultiplexer.Connect(server);
            _db = redis.GetDatabase();
        }
        public bool Save(string key, string value)
        {
            Log.Debug("Saving data to Redis into {0} list", key);
            _db.ListLeftPush(key, value);
            return true;
        }
    }
}
