using System.Runtime.CompilerServices;
using System.Text;
using PasswordStorageManager.Core.Interfaces.Services;
using PasswordStorageManager.Core.Models;

namespace PasswordStorageManager;

public class PasswordManager
{

    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly IVaultService _vaultService;
    private UserModel _usermodel;

    public PasswordManager(IAuthService authService, IUserService userService, IVaultService vaultService)
    {
        _authService = authService;
        _userService = userService;
        _vaultService = vaultService;
        Start();
    }

    private void Start()
    {
        Console.WriteLine("Welcome to your personal password storage manager."+"Choose an option:"+"\n 1: Create new user"+"\n 2: Login");
        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                CreateNewUser();
                break;
            case "2":
                Login();
                break;
        }
    }
    

    private void Login()
    {
        Console.WriteLine("Username: ");
        var username = Console.ReadLine();
        
        Console.WriteLine("Password: ");
        var pass = ReadPassword();
        try
        {
            var user = _userService.GetAllUsers().FirstOrDefault(u => u.Username == username);
            if (user == null) Console.WriteLine("Username or password incorrect");
            var authenticated = _authService.AuthenticateLogin(user!, pass!);
            if (!authenticated) Console.WriteLine("Username or password incorrect");
                
            else
            {
                _usermodel = user!;
                Menu();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Username or password incorrect");
            throw;
        }
    }

    private void CreateNewUser()
    {
        Console.WriteLine("select Username: ");
        var username = Console.ReadLine();
        Console.WriteLine("Master Password: ");
        var mPassword = ReadPassword();

        try
        {
            var newUser = new UserModel
            {
                Username = username!,
                Password = mPassword!
            };
            if (newUser.isPasswordValid())
            {
                var user = _authService.PasswordHasher(username!, mPassword!);
                _userService.CreateUser(user);
                Thread.Sleep(3000);
                Login();
            }
            else
            {
                Console.WriteLine("You password does not meet the requirements, try again");
                Thread.Sleep(2000);
                CreateNewUser();
            }
        } 
        catch (Exception e)
        {
            Console.WriteLine("Something went very wrong :(");
            throw;
        }
    }

    private static string ReadPassword()
    {
        var password = "";
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[0..^1];
                Console.Write("\b \b");
            } else if (!char.IsControl(key.KeyChar))
            {
                password += key.KeyChar;
                Console.Write("*");
            }
        } while (key.Key != ConsoleKey.Enter);
        return password;
    }

    private void Menu()
    {
        Console.Clear();
        Console.WriteLine("What would you like to do?");
        Console.WriteLine("\n 1: View stored items \n 2: Create a new item \n 3: Logout");
        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                ViewItems();
                break;
            case "2":
                CreateItem();
                break;
            case "3":
                Console.Clear();
                Environment.Exit(0);
                break;
        }
    }

    private void ViewItems()
    {
        var items = _vaultService.GetItemsByUserId(_usermodel.Id).ToList();
        foreach (var item in items)
        {
            var salt = Encoding.ASCII.GetString(_usermodel.PasswordSalt);
            var key = salt.Substring(0, 32);
            Console.WriteLine($"{item.ItemName}");
            Console.WriteLine($" Username : {item.Username} ");
            Console.WriteLine($" Password : {_authService.GetDecryptedPassword(key, item.EncryptedPassword)}");
        }
        Console.WriteLine("1 to go back to the main menu or write 2 to exit the app.");
        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                Menu();
                break;
            case "2":
                Console.Clear(); 
                Environment.Exit(0);
                break;
        }
    }

    private void CreateItem()
    {
        var password = "";
        Console.WriteLine("Name your item:");
        var itemName = Console.ReadLine();
        Console.WriteLine("Username: ");
        var username = Console.ReadLine();
        Console.WriteLine("Do you want to generate a strong password? yes/no?");
        var generatePass = Console.ReadLine().Trim().ToLower();
        switch (generatePass)
        {
            case "yes":
                password = GeneratePassword();
                break;
            case "no":
            {
                Console.WriteLine("Type your password:");
                password = ReadPassword();
                break;
            }
        }

        var salt = Encoding.ASCII.GetString(_usermodel.PasswordSalt);
        var key = salt.Substring(0, 32);
        var encryptedPassword = _authService.EncryptItemPassword(key, password);
        
        var newItem = new ItemModel
        {
            ItemName = itemName!,
            Username = username!,
            EncryptedPassword = encryptedPassword,
            UserId = _usermodel.Id
        };

        _vaultService.SaveItem(newItem);
        
        Console.WriteLine("Your item has been safely stored");
        Thread.Sleep(2000);
        Menu();
    }

    public static string GeneratePassword()
    {
        var length = 18;
        const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$";
        var random = new Random();
        var passwordChars = new char[length];

        for (int i = 0; i < length; i++)
        {
            passwordChars[i] = validChars[random.Next(0, validChars.Length)];
        }

        return new string(passwordChars);
    }
}