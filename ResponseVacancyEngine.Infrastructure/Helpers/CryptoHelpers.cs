using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using ResponseVacancyEngine.Application.Interfaces.CryptoHelper;
using ResponseVacancyEngine.Infrastructure.Options;

namespace ResponseVacancyEngine.Infrastructure.Helpers;

public class CryptoHelper : ICryptoHelper
{
    private readonly IOptions<CryptoOptions> _options;
    private readonly IDataProtector _protector;

    public CryptoHelper(IOptions<CryptoOptions> options, IDataProtectionProvider provider)
    {
        _options = options;
        _protector = provider.CreateProtector(options.Value.Key);
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        return _protector.Protect(plainText);
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        return _protector.Unprotect(cipherText);
    }
}