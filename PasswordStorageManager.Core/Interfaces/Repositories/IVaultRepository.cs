using PasswordStorageManager.Core.Models;

namespace PasswordStorageManager.Core.Interfaces.Repositories;

public interface IVaultRepository
{
    void SaveItem(ItemModel newItem);
    IEnumerable<ItemModel> GetItemsByUserId(string userId);
}