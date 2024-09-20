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

// Move configuration to extensions in format of methodes in IServiceCollections
// we used extension to add methodes to generate mongorepository  and generate mongo client. As we can use it for adding other services collections and repositories
builder.Services.AddMongo(builder.Configuration)
                .AddMongoRepository<Item>("items");

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

