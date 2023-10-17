using PasswordStorageManager.Core.Models;

namespace PasswordStorageManager.Core.Interfaces.Services;

public interface IUserService
{
    IEnumerable<UserModel> GetAllUsers();
    UserModel GetUser(string email);
    public void CreateUser(UserModel user);
}