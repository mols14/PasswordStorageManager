using System.Security.Authentication.ExtendedProtection;
using Microsoft.Extensions.DependencyInjection;
using PasswordStorageManager.Core.Interfaces.Repositories;
using PasswordStorageManager.Core.Interfaces.Services;
using PasswordStorageManager.Core.Services;
using PasswordStorageManager.infrastructure.Repositories;

namespace PasswordStorageManager
{


    public class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services
                .AddSingleton<IAuthService, AuthService>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<IVaultService, VaultService>()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<IVaultRepository, VaultRepository>()
                .AddSingleton<IPasswordRepository, PasswordRepository>()
                .AddSingleton<PasswordManager, PasswordManager>()
                .BuildServiceProvider().GetService<PasswordManager>();

        }
    }
}