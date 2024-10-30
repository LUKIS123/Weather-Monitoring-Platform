namespace WeatherMonitorCore.Interfaces;
public interface IAesEncryptionHelper
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}
