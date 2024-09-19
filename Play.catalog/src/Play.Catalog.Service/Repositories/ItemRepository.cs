using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        // private const string collectionName = "items";  // mongoDb collection name
        private readonly IMongoCollection<T> dbCollection; // mongoDb collection
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter; // filter builder

        public MongoRepository( IMongoDatabase database, string collectionName )
        {
            /* without dependency injection */
            // var mongoClient = new MongoClient("mongodb://localhost:27017"); // mongoDb client to create a connection to a MongoDB server
            // var database = mongoClient.GetDatabase("Catalog"); // Retrieves an instance of the Catalog database from the MongoDB server
            
            dbCollection = database.GetCollection<T>(collectionName); // Stores and retrieves documents that follow the structure of the Item class
        }

        // Corrected method name
        public async Task<IReadOnlyCollection<T>> GetAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<T> GetAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await dbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            FilterDefinition<T> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);

            await dbCollection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);

            await dbCollection.DeleteOneAsync(filter);
        }
    }

}