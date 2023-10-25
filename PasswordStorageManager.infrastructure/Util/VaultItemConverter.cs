using PasswordStorageManager.Core.Models;
using PasswordStorageManager.Core.Schemas;

namespace PasswordStorageManager.infrastructure.Util;

public class VaultItemConverter
{
    public IEnumerable<ItemModel> Convert(List<VaultItem> items, List<SecureItemKeys> keys)
    {
        var combinedItems = items.Select(vault => new ItemModel
        {
            ItemName = vault.ItemName,
            Username = vault.Username,
            EncryptedPassword = vault.EncryptedPassword,
            UserId = vault.UserId,
            IV = keys.FirstOrDefault(key => key.ItemId == vault.Id.ToString())!.IV
        });
        return combinedItems;
    }
}