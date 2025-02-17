namespace Kryptograf.Services;

public interface IEncryptionService
{
    string Encrypt(string text, string password);
    string Decrypt(string cipherText, string password);
}
