using MongoDB.Driver;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Seed
{
    public class DatabaseSeeder
    {
        private readonly IMongoCollection<Property> _properties;
        private readonly IMongoCollection<Owner> _owners;

        public DatabaseSeeder(IMongoDatabase database)
        {
            _properties = database.GetCollection<Property>("Properties");
            _owners = database.GetCollection<Owner>("Owners");
        }

        public async Task SeedAsync()
        {
            // Seed Owners
            if (await _owners.CountDocumentsAsync(_ => true) == 0)
            {
                var sampleOwners = new List<Owner>
                {
                    new Owner { FullName = "Juan Pérez", Email = "juan.perez@example.com", Phone = "3001234567" },
                    new Owner { FullName = "María Gómez", Email = "maria.gomez@example.com", Phone = "3109876543" },
                    new Owner { FullName = "Carlos López", Email = "carlos.lopez@example.com", Phone = "3014567890" }
                };

                await _owners.InsertManyAsync(sampleOwners);
            }

            // Seed Properties solo si está vacío
            if (await _properties.CountDocumentsAsync(_ => true) == 0)
            {
                var owners = await _owners.Find(_ => true).ToListAsync();

                var sampleProperties = new List<Property>
                {
                    new Property
                    {
                        IdOwner = owners[0].Id,
                        Name = "Casa en Medellín",
                        Address = "Cra 50 #10-20, Medellín",
                        Price = 350000000,
                        ImageUrl = "https://picsum.photos/400/300?random=1"
                    },
                    new Property
                    {
                        IdOwner = owners[1].Id,
                        Name = "Apartamento en Bogotá",
                        Address = "Calle 100 #15-30, Bogotá",
                        Price = 450000000,
                        ImageUrl = "https://picsum.photos/400/300?random=2"
                    },
                    new Property
                    {
                        IdOwner = owners[2].Id,
                        Name = "Finca en el Quindío",
                        Address = "Km 8 Vía Armenia - Pereira, Quindío",
                        Price = 600000000,
                        ImageUrl = "https://picsum.photos/400/300?random=3"
                    }
                };

                await _properties.InsertManyAsync(sampleProperties);
            }
        }
    }
}
