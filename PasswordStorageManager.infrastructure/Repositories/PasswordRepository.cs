using MongoDB.Driver;
using PasswordStorageManager.Core.Interfaces.Repositories;
using PasswordStorageManager.Core.Schemas;
using PasswordStorageManager.infrastructure.Util;
using Microsoft.Extensions.Configuration;

namespace PasswordStorageManager.infrastructure.Repositories;

public class PasswordRepository : IPasswordRepository
{
    private readonly IMongoCollection<User> _users;
    private readonly UserConverter _converter;
    private readonly IConfiguration _config;
    
    public PasswordRepository()
    {
        _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        
        _converter = new UserConverter();

        var settings = MongoClientSettings.FromConnectionString(_config.GetConnectionString("MongoDB"));
        var client = new MongoClient(settings);
        var db = client.GetDatabase("PasswordStorageManager");

        _users = db.GetCollection<User>("Users");
    }
}