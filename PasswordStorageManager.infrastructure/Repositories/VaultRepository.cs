using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using PasswordStorageManager.Core.Interfaces.Repositories;
using PasswordStorageManager.Core.Models;
using PasswordStorageManager.Core.Schemas;
using PasswordStorageManager.infrastructure.Util;

namespace PasswordStorageManager.infrastructure.Repositories;

public class VaultRepository : IVaultRepository
{
    private readonly IMongoCollection<VaultItem> _vaultItems;
    private readonly IMongoCollection<SecureItemKeys> _itemKeys;
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
        var passwordStorageDb = client.GetDatabase("PasswordStorageManager");
        var encryptionParamsDb = client.GetDatabase("EncryptionParameters");

        _vaultItems = passwordStorageDb.GetCollection<VaultItem>("VaultItems");
        _itemKeys = encryptionParamsDb.GetCollection<SecureItemKeys>("SecureItemKeys");
    }

    public void SaveItem(ItemModel newItem)
    {
        var itemToInsert = new VaultItem
        {
            ItemName = newItem.ItemName,
            Username = newItem.Username,
            EncryptedPassword = newItem.EncryptedPassword,
            UserId = newItem.UserId,
            Id = ObjectId.GenerateNewId()
        };
        _vaultItems.InsertOne(itemToInsert);
        _itemKeys.InsertOne( new SecureItemKeys
        {
            ItemId = itemToInsert.Id.ToString()!,
            UserId = newItem.UserId,
            IV = newItem.IV
        });
    }

    public IEnumerable<ItemModel> GetItemsByUserId(string userId)
    {
        try
        {
            var vaultFilter = Builders<VaultItem>.Filter.Eq(v => v.UserId, userId);
            var secureKeyFilter = Builders<SecureItemKeys>.Filter.Eq(k => k.UserId, userId);
            
            var items = _vaultItems.Find(vaultFilter).ToList();
            var secureKeys = _itemKeys.Find(secureKeyFilter).ToList();
            
            return _converter.Convert(items, secureKeys);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}