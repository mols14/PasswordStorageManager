using PasswordStorageManager.Core.Models;
using PasswordStorageManager.Core.Schemas;

namespace PasswordStorageManager.infrastructure.Util;

public class VaultItemConverter
{
        public ItemModel Convert(VaultItem model)
        {
            return new ItemModel()
            {
                ItemName = model.ItemName,
                Username = model.Username,
                EncryptedPassword = model.EncryptedPassword,
                UserId = model.UserId
            };
        }
}