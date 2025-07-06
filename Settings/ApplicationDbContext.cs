using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace DAMApi.Settings
{
    public class ApplicationDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly GridFSBucket _gridbucket;
        private readonly ILogger<ApplicationDbContext> _logger;
        public ApplicationDbContext(IOptions<MongoDbSettings> MongoDbSettings, ILogger<ApplicationDbContext> logger)
        {

            _logger = logger;
            //create a mongo client
            var client = new MongoClient(MongoDbSettings.Value.ConnectionString);
            //get the database
            _database = client.GetDatabase(MongoDbSettings.Value.DatabaseName);
            //initialize GridFSBucket for file storage
            _gridbucket = new GridFSBucket(_database);

        }
        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
        public GridFSBucket GridFsBucket => _gridbucket;
    }
}



