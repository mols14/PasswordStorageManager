using PasswordStorageManager.Core.Models;

namespace PasswordStorageManager.Core.Interfaces.Repositories;

public interface IVaultRepository
{
    IEnumerable<ItemModel> GetItemsByUserId(string userId);
}