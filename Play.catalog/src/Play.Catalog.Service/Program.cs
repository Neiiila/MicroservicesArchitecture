using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;
using Play.Catalog.Service.Settings;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(
    // if we call an async method in our code, the method name will have Async suffix
    options =>
    {
        options.SuppressAsyncSuffixInActionNames = false;
    }

);

// Bind settings from appsettigns : Configure the services with the settings in appSettings.json file
builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection(nameof(ServiceSettings)));
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection(nameof(MongoDbSettings)));

builder.Services.AddSingleton( serviceProvider =>
{
    /* create a mongo client to add it to my services  */
    // Retrieve MongoDbSettings
    var mongoDbSettings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    // Create MongoClient
    var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);

    // Retrieve ServiceSettings
    var serviceSettings = serviceProvider.GetRequiredService<IOptions<ServiceSettings>>().Value;

    // Return MongoDB database instance
    return mongoClient.GetDatabase(serviceSettings.ServiceName);
});

builder.Services.AddSingleton<IRepository<Item>>(serviceProvider => {
    // get an instance that already registred in container 
    var database = serviceProvider.GetRequiredService<IMongoDatabase>();
    return new MongoRepository<Item>(database, "items"); // geenerate a repository for the Item entity, where we give it databse and collection name 
});

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// equivalent to Service.http that does the mapping 
app.UseRouting();

app.MapControllers();


app.Run();

