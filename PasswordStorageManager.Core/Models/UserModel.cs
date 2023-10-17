namespace PasswordStorageManager.Core.Models;

public class UserModel
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public bool isPasswordValid()
    {
        return Password.Length > 8 &&
               Password.Any(char.IsUpper) &&
               Password.Any(char.IsLower) &&
               Password.Any(char.IsDigit) &&
               Password.Any(ch => "!@#$%^&*()".Contains(ch));
    }
}