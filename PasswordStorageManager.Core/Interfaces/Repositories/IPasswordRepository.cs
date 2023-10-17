using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using PasswordStorageManager.Core.Schemas;

namespace PasswordStorageManager.Core.Interfaces.Repositories;

public interface IPasswordRepository
{
    public PasswordRepository()
    {
        _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        
        _converter = new UserConverter();

        var settings = MongoClientSettings.FromConnectionString(_config.GetConnectionString("MongoDB"));
        var client = new MongoClient(settings);
        var db = client.GetDatabase("PasswordManager");

        _users = db.GetCollection<User>("Users");
    }
}