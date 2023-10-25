using PasswordStorageManager.Core.Models;
using PasswordStorageManager.Core.Schemas;

namespace PasswordStorageManager.infrastructure.Util;

public class UserConverter
{
    public IEnumerable<UserModel> Convert(List<User> users, List<SecureUserKeys> keys)
    {
        var combinedUsers = users.Select(user => new UserModel
        {
            Id = user.Id.ToString()!,
            Username = user.Username,
            EncryptedRandom = user.EncryptedRandom,
            IV = keys.FirstOrDefault(k => k.UserId == user.Id.ToString())!.IV,
            PasswordSalt = keys.FirstOrDefault(k => k.UserId == user.Id.ToString())!.PasswordSalt
        });
        return combinedUsers;
    }
}