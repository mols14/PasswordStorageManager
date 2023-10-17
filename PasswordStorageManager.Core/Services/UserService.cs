using PasswordStorageManager.Core.Interfaces.Repositories;
using PasswordStorageManager.Core.Interfaces.Services;
using PasswordStorageManager.Core.Models;

namespace PasswordStorageManager.Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }
    
    public IEnumerable<UserModel> GetAllUsers()
    {
        return _repo.GetAllUsers();
    }

    public UserModel GetUser(string email)
    {
        return _repo.GetUser(email);
    }
    
    public void CreateUser(UserModel user)
    {
        if (user == null)
        {
            throw new ArgumentException();
        }

        _repo.CreateUser(user);
    }
}