using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{
    public class ItemRepository 
    {
        private const string  collectionName = "items";  // mongoDb collection name 

        private readonly IMongoCollection<Item> dbCollection ; // mongoDb collection

        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter; // filter builder
    
        public ItemRepository()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017"); // mongoDb client To create a connection to a MongoDB server
            var database = mongoClient.GetDatabase("Catalog"); // it retriec eand instace of the catalog database from mongodb server
            dbCollection = database.GetCollection<Item>(collectionName); // it woll store and retriece documents that follow the structure of Item class
        }
        
        public async Task<IReadOnlyCollection<Item>> GetAllAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<Item> GetItemById( Guid id ){

            FilterDefinition<Item> filter = filterBuilder.Eq( item => item.Id, id );
            return await dbCollection.Find(filter).FirstOrDefaultAsync();  
        }

        public async Task CreateItemAsync( Item item ){
            
            if( item == null )
            {
                throw new ArgumentNullException(nameof(item));
            }

            await dbCollection.InsertOneAsync(item) ; 
        }

        public async Task UpdateItemAsync( Item item ){
            
            if( item == null )
            {
                throw new ArgumentNullException(nameof(item));
            }
            
            FilterDefinition<Item> filter = filterBuilder.Eq( existingItem => existingItem.Id, item.Id );          

            await dbCollection.ReplaceOneAsync(filter, item);                                                     
        }

        public async Task DeleteItemAsync( Guid id ) {
            FilterDefinition<Item> filter = filterBuilder.Eq( item => item.Id , id ) ; 

            await dbCollection.DeleteOneAsync( filter ); 
        } 
    }
}