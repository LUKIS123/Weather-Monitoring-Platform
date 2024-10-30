using System.Security.Cryptography;
using WeatherMonitorCore.Interfaces;

namespace WeatherMonitorCore.Infrastructure.Utility;

internal class AesEncryptionHelper : IAesEncryptionHelper
{
    private readonly EncryptionSettings _encryptionSettings;

    public AesEncryptionHelper(EncryptionSettings encryptionSettings)
    {
        _encryptionSettings = encryptionSettings;
    }

    public string Encrypt(string plainText)
    {
        using var aesAlg = Aes.Create();
        aesAlg.Key = Convert.FromBase64String(_encryptionSettings.Key);
        aesAlg.IV = Convert.FromBase64String(_encryptionSettings.IV);

        using var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }
        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        using var aesAlg = Aes.Create();
        aesAlg.Key = Convert.FromBase64String(_encryptionSettings.Key);
        aesAlg.IV = Convert.FromBase64String(_encryptionSettings.IV);

        using var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        using var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText));
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        return srDecrypt.ReadToEnd();
    }
}

internal class EncryptionSettings
{
    public string Key { get; set; } = null!;
    public string IV { get; set; } = null!;
}
