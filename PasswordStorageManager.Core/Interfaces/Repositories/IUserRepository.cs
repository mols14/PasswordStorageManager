using PasswordStorageManager.Core.Models;

namespace PasswordStorageManager.Core.Interfaces.Repositories;

public interface IUserRepository
{
    IEnumerable<UserModel> GetAllUsers();
    void CreateUser(UserModel user);
}