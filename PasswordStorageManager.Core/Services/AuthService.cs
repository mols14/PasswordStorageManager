using System.Security.Cryptography;
using System.Text;
using PasswordStorageManager.Core.Interfaces.Services;
using PasswordStorageManager.Core.Models;

namespace PasswordStorageManager.Core.Services;

public class AuthService : IAuthService
{
    public UserModel PasswordHasher(string username, string password)
    {
        GenerateUserCredentials(password, out var passwordHash, out var passwordSalt);

        var user = new UserModel
        {
            Username = username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };
        return user;
    }

    public byte[] EncryptItemPassword(string key, string plaintextpass)
    {
        if (string.IsNullOrEmpty(plaintextpass) || string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key and plaintext password cannot be empty.");
        }
        
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintextpass);
        
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.KeySize = 256;
            aesAlg.IV = GenerateRandomIV(); // Best Practice
            
            using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(keyBytes, aesAlg.IV))
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream csEncrypt = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                csEncrypt.Write(plaintextBytes, 0, plaintextBytes.Length);
                csEncrypt.FlushFinalBlock();
                return ms.ToArray();
            }
        }
    }

    private byte[] GenerateRandomIV()
    {
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            byte[] iv = new byte[16];
            rng.GetBytes(iv);
            return iv;
        }
    }

    public bool AuthenticateLogin(UserModel user, string providedPassword)
    {
        using var df2 = new Rfc2898DeriveBytes(providedPassword, user.PasswordSalt, 10000, HashAlgorithmName.SHA512);
        var calculatedHash = df2.GetBytes(64);

        return CompareByteArrays(calculatedHash, user.PasswordHash);
    }

    public string GetDecryptedPassword(string key, byte[] encryptedPassword)
    {
        using var aes = Aes.Create();

        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = new byte[16];

        var decrypt = aes.CreateDecryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream(encryptedPassword);
        using var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }

    private bool CompareByteArrays(byte[] calculatedHash, byte[] userPasswordHash)
    {
        if (calculatedHash.Length != userPasswordHash.Length)
        {
            return false;
        }

        return !calculatedHash.Where((t, i) => t != userPasswordHash[i]).Any();
    }
    private void GenerateUserCredentials(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var rng = new RNGCryptoServiceProvider();
        var salt = new byte[32];
        rng.GetBytes(salt);

        
        
        using var df2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA512);
        
        passwordHash = df2.GetBytes(64);
        passwordSalt = salt;
    }
}