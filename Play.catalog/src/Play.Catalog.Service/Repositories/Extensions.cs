using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Settings;

namespace Play.Catalog.Service.Repositories
{
    // Extensions are static classes that can contain static methods with static members
    public static class Extensions
    {
        // this, means that we entend IserviceCollection with a new method AddMongo
        public static IServiceCollection AddMongo ( this IServiceCollection services, IConfiguration configuration ){
            // Bind settings from appsettigns : Configure the services with the settings in appSettings.json file
            services.Configure<ServiceSettings>(configuration.GetSection(nameof(ServiceSettings)));
            services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));

            services.AddSingleton( serviceProvider =>
            {
                /* create a mongo client to add it to my services  */
                
                var configuration = serviceProvider.GetService<IConfiguration>(); 
                // Retrieve MongoDbSettings
                var mongoDbSettings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                // Create MongoClient
                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);

                // Retrieve ServiceSettings
                var serviceSettings = serviceProvider.GetRequiredService<IOptions<ServiceSettings>>().Value;

                // Return MongoDB database instance
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });
            return services;
        }
         public static IServiceCollection  AddMongoRepository<T>(this IServiceCollection services, string collectionName) where T : IEntity
         {
            services.AddSingleton<IRepository<T>>(serviceProvider => {
                // get an instance that already registred in container 
                var database = serviceProvider.GetRequiredService<IMongoDatabase>();
                return new MongoRepository<T>(database, collectionName); // geenerate a repository for the Item entity, where we give it databse and collection name 
            });
            return services;
         }
    }

   
}