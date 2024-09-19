using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{
    public class ItemRepository : IItemsRepository
    {
        private const string collectionName = "items";  // mongoDb collection name
        private readonly IMongoCollection<Item> dbCollection; // mongoDb collection
        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter; // filter builder

        public ItemRepository( IMongoDatabase database )
        {
            /* without dependency injection */
            // var mongoClient = new MongoClient("mongodb://localhost:27017"); // mongoDb client to create a connection to a MongoDB server
            // var database = mongoClient.GetDatabase("Catalog"); // Retrieves an instance of the Catalog database from the MongoDB server
            
            dbCollection = database.GetCollection<Item>(collectionName); // Stores and retrieves documents that follow the structure of the Item class
        }

        // Corrected method name
        public async Task<IReadOnlyCollection<Item>> GetItemsAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            FilterDefinition<Item> filter = filterBuilder.Eq(item => item.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateItemAsync(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await dbCollection.InsertOneAsync(item);
        }

        public async Task UpdateItemAsync(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            FilterDefinition<Item> filter = filterBuilder.Eq(existingItem => existingItem.Id, item.Id);

            await dbCollection.ReplaceOneAsync(filter, item);
        }

        public async Task DeleteItemAsync(Guid id)
        {
            FilterDefinition<Item> filter = filterBuilder.Eq(item => item.Id, id);

            await dbCollection.DeleteOneAsync(filter);
        }
    }

}