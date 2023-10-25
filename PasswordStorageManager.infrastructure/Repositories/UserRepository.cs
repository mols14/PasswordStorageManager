using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using PasswordStorageManager.Core.Interfaces.Repositories;
using PasswordStorageManager.Core.Models;
using PasswordStorageManager.Core.Schemas;
using PasswordStorageManager.infrastructure.Util;

namespace PasswordStorageManager.infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;
    private readonly UserConverter _converter;
    private readonly IConfigurationRoot _config;
    private readonly IMongoCollection<SecureUserKeys> _userKeys;

    public UserRepository()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var configPath = Path.Combine(basePath, "../../../../PasswordStorageManager.infrastructure/appsettings.json");

        _config = new ConfigurationBuilder()
            .AddJsonFile(configPath)
            .Build();
        
        _converter = new UserConverter();
        var settings = MongoClientSettings.FromConnectionString(_config.GetConnectionString("MongoDB"));

        var client = new MongoClient(settings);
        var passwordDb = client.GetDatabase("PasswordStorageManager");
        var encryptionParamsDb = client.GetDatabase("EncryptionParameters");

        _users = passwordDb.GetCollection<User>("Users");
        _userKeys = encryptionParamsDb.GetCollection<SecureUserKeys>("SecureUserKeys");
    }
    
    public IEnumerable<UserModel> GetAllUsers()
    {
        try
        {
            var users = _users.Find(_ => true).ToList();
            var secureKeys = _userKeys.Find(_ => true).ToList();

            return _converter.Convert(users, secureKeys);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public void CreateUser(UserModel user)
    {
        try
        {
            var insertOne = new User
            {
                Username = user.Username,
                EncryptedRandom = user.EncryptedRandom,
                Id = ObjectId.GenerateNewId()
            };
            _users.InsertOne(insertOne);
            _userKeys.InsertOne(new SecureUserKeys
            {
                UserId = insertOne.Id.ToString()!,
                IV = user.IV,
                PasswordSalt = user.PasswordSalt
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}