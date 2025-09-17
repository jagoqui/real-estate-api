using MongoDB.Driver;
using RealEstate.Domain.Entities;
using System.Text.Json;

namespace RealEstate.Infrastructure.Seed
{
    public class DatabaseSeeder
    {
        private readonly IMongoDatabase _database;

        public DatabaseSeeder(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task SeedAsync()
        {
            await SeedCollectionAsync<Owner>("Owners", "owners.json");
            await SeedCollectionAsync<Property>("Properties", "properties.json");
            await SeedCollectionAsync<PropertyImage>("PropertyImages", "propertyImages.json");
            await SeedCollectionAsync<PropertyTrace>("PropertyTraces", "propertyTraces.json");
        }

        private async Task SeedCollectionAsync<T>(string collectionName, string fileName)
        {
            var collection = _database.GetCollection<T>(collectionName);

            if (await collection.CountDocumentsAsync(_ => true) == 0)
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "Infrastructure", "Data", "SeedData", fileName);

                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Seed file not found: {filePath}");

                var jsonData = await File.ReadAllTextAsync(filePath);
                var records = JsonSerializer.Deserialize<List<T>>(jsonData);

                if (records != null && records.Count > 0)
                {
                    if (typeof(T) == typeof(Property))
                    {
                        var owners = await _database.GetCollection<Owner>("Owners")
                            .Find(_ => true).ToListAsync();

                        var props = records.Cast<Property>().ToList();

                        for (int i = 0; i < props.Count; i++)
                        {
                            props[i].IdOwner = owners[i % owners.Count].IdOwner;
                        }

                        await _database.GetCollection<Property>("Properties").InsertManyAsync(props);
                    }
                    else if (typeof(T) == typeof(PropertyImage))
                    {
                        var properties = await _database.GetCollection<Property>("Properties")
                            .Find(_ => true).ToListAsync();

                        var images = records.Cast<PropertyImage>().ToList();

                        for (int i = 0; i < images.Count; i++)
                        {
                            images[i].IdProperty = properties[i % properties.Count].IdProperty;
                        }

                        await _database.GetCollection<PropertyImage>("PropertyImages").InsertManyAsync(images);
                    }
                    else if (typeof(T) == typeof(PropertyTrace))
                    {
                        var properties = await _database.GetCollection<Property>("Properties")
                            .Find(_ => true).ToListAsync();

                        var traces = records.Cast<PropertyTrace>().ToList();

                        for (int i = 0; i < traces.Count; i++)
                        {
                            traces[i].IdProperty = properties[i % properties.Count].IdProperty;
                        }

                        await _database.GetCollection<PropertyTrace>("PropertyTraces").InsertManyAsync(traces);
                    }
                    else
                    {
                        await collection.InsertManyAsync(records);
                    }
                }
            }
        }
    }
}
