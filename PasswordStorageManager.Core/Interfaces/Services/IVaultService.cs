using PasswordStorageManager.Core.Models;

namespace PasswordStorageManager.Core.Interfaces.Services;

public interface IVaultService
{
    void SaveItem(ItemModel newItem);
    IEnumerable<ItemModel> GetItemsByUserId(string userId);
}