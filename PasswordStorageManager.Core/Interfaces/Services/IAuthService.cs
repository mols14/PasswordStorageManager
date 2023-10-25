using PasswordStorageManager.Core.Models;

namespace PasswordStorageManager.Core.Interfaces.Services;

public interface IAuthService
{
    UserModel PasswordHasher(string username, string password);
    bool AuthenticateLogin(UserModel user, string providedPassword);
    string GetDecryptedPassword(ItemModel item);
    void EncryptItemPassword(ItemModel newItem, string plaintextpass);
}