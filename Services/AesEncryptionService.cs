using System.Security.Cryptography;
using System.Text;

namespace Kryptograf.Services;

public class AesEncryptionService : IEncryptionService
{
    private const int SaltSize = 16;
    private const int KeySize = 32; // 256-bit AES
    private const int Iterations = 10000;

    public string Encrypt(string text, string password)
    {
        var salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using (var aes = Aes.Create())
        {
            aes.GenerateIV();
            using (var keyDerivation = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                aes.Key = keyDerivation.GetBytes(KeySize);
            }

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                var textBytes = Encoding.UTF8.GetBytes(text);
                var encryptedBytes = encryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);

                var result = new byte[salt.Length + aes.IV.Length + encryptedBytes.Length];
                Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
                Buffer.BlockCopy(aes.IV, 0, result, salt.Length, aes.IV.Length);
                Buffer.BlockCopy(encryptedBytes, 0, result, salt.Length + aes.IV.Length, encryptedBytes.Length);

                return Convert.ToBase64String(result);
            }
        }
    }

    public string Decrypt(string cipherText, string password)
    {
        if (string.IsNullOrEmpty(cipherText))
            return null;

        byte[] fullCipher;
        try
        {
            fullCipher = Convert.FromBase64String(cipherText);
        }
        catch (FormatException)
        {
            return null; // Niepoprawny Base64
        }

        if (fullCipher.Length < SaltSize * 2)
            return null; // Zbyt krótki szyfrogram

        var salt = new byte[SaltSize];
        var iv = new byte[SaltSize];
        var cipherBytes = new byte[fullCipher.Length - salt.Length - iv.Length];

        Buffer.BlockCopy(fullCipher, 0, salt, 0, SaltSize);
        Buffer.BlockCopy(fullCipher, SaltSize, iv, 0, SaltSize);
        Buffer.BlockCopy(fullCipher, SaltSize * 2, cipherBytes, 0, cipherBytes.Length);

        using (var aes = Aes.Create())
        {
            using (var keyDerivation = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                aes.Key = keyDerivation.GetBytes(KeySize);
            }

            aes.IV = iv;

            try
            {
                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    var decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
            catch (CryptographicException)
            {
                return null; // Błędny klucz lub uszkodzone dane
            }
        }
    }
}
