namespace ResponseVacancyEngine.Application.Infrastructure.Interfaces.CryptoHelper;

public interface ICryptoHelper
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}