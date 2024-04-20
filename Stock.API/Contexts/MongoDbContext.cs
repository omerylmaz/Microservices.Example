using MongoDB.Driver;

namespace Stock.API.Contexts
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _mongoDatabase;
        public MongoDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            _mongoDatabase = client.GetDatabase("StockDb");
        }

        public IMongoCollection<T> GetCollection<T>() => _mongoDatabase.GetCollection<T>(typeof(T).Name.ToLowerInvariant());

        public async void SeedData()
        {
            var collection = GetCollection<Stock.API.Models.Entities.Stock>();
            //var deneme = collection.FindSync(s => true).ToList();
            if (!collection.FindSync(s => true).Any()) 
            {
                await collection.InsertManyAsync(new List<Stock.API.Models.Entities.Stock>
                {
                    new Stock.API.Models.Entities.Stock
                    {
                        ProductId = Guid.NewGuid(),
                        Count = 10
                    },
                    new Stock.API.Models.Entities.Stock
                    {
                        ProductId = Guid.NewGuid(),
                        Count = 20
                    },
                    new Stock.API.Models.Entities.Stock
                    {
                        ProductId = Guid.NewGuid(),
                        Count = 30
                    }
                });
            }
        }
    }
}
