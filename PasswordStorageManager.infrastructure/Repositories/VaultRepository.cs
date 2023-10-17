using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using PasswordStorageManager.Core.Interfaces.Repositories;
using PasswordStorageManager.Core.Models;
using PasswordStorageManager.Core.Schemas;
using PasswordStorageManager.infrastructure.Util;

namespace PasswordStorageManager.infrastructure.Repositories;

public class VaultRepository : IVaultRepository
{
    private readonly IMongoCollection<VaultItem> _vaultItems;
    private readonly VaultItemConverter _converter;
    private readonly IConfigurationRoot _config;

    public VaultRepository()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var configPath = Path.Combine(basePath, "../../../../PasswordStorageManager.infrastructure/appsettings.json");

        _config = new ConfigurationBuilder().
            AddJsonFile(configPath)
            .Build();

        
        _converter = new VaultItemConverter();
       
        var settings = MongoClientSettings.FromConnectionString(_config.GetConnectionString("MongoDB"));
        var client = new MongoClient(settings);
        var db = client.GetDatabase("PasswordStorageManager");

        _vaultItems = db.GetCollection<VaultItem>("VaultItems");
    }

    public void SaveItem(ItemModel newItem)
    {
        Console.WriteLine(newItem.ItemName, newItem.EncryptedPassword, newItem.Username, newItem.UserId);
        Thread.Sleep(5);
        _vaultItems.InsertOne(new VaultItem
        {
            ItemName = newItem.ItemName,
            Username = newItem.Username,
            EncryptedPassword = newItem.EncryptedPassword,
            UserId = newItem.UserId
        });
    }

    public IEnumerable<ItemModel> GetItemsByUserId(string userId)
    {
        try
        {
            var filter = Builders<VaultItem>.Filter.Eq(v => v.UserId, userId);
            var items = _vaultItems.Find(filter).ToList();
            return items.Select(vault => _converter.Convert(vault));

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}