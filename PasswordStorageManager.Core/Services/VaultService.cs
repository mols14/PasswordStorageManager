using PasswordStorageManager.Core.Interfaces.Repositories;
using PasswordStorageManager.Core.Interfaces.Services;
using PasswordStorageManager.Core.Models;

namespace PasswordStorageManager.Core.Services;

public class VaultService : IVaultService
{
    private readonly IVaultRepository _repo;
    public VaultService(IVaultRepository repo)
    {
        _repo = repo;
    }
    public void SaveItem(ItemModel newItem)
    {
        _repo.SaveItem(newItem);
    }

    public IEnumerable<ItemModel> GetItemsByUserId(string userId)
    {
        return _repo.GetItemsByUserId(userId);
    }
}