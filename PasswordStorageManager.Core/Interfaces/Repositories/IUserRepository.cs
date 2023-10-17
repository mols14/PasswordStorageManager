using PasswordStorageManager.Core.Models;

namespace PasswordStorageManager.Core.Interfaces.Repositories;

public interface IUserRepository
{
    IEnumerable<UserModel> GetAllUsers();
    UserModel GetUser(string email);
    void CreateUser(UserModel user);
}