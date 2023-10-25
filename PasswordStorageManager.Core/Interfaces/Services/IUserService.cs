using PasswordStorageManager.Core.Models;

namespace PasswordStorageManager.Core.Interfaces.Services;

public interface IUserService
{
    IEnumerable<UserModel> GetAllUsers();
    public void CreateUser(UserModel user);
}