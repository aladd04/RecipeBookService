using System;
using System.Security.Cryptography;
using System.Text;

namespace RecipeBookApi.Factories;

internal static class CryptoFactory
{
    public static string Encrypt(string key, string dataToEncrypt)
    {
        var cryptoServiceProvider = GetCryptoServiceProvider(key);
        var dataToEncryptBytes = Encoding.UTF8.GetBytes(dataToEncrypt);

        var resultsBytes = cryptoServiceProvider.CreateEncryptor().TransformFinalBlock(dataToEncryptBytes, 0, dataToEncryptBytes.Length);

        return Convert.ToBase64String(resultsBytes, 0, resultsBytes.Length);
    }

    public static string Decrypt(string key, string cipherToDecrypt)
    {
        var cryptoServiceProvider = GetCryptoServiceProvider(key);
        var cipherToDecryptBytes = Convert.FromBase64String(cipherToDecrypt);

        var resultsBytes = cryptoServiceProvider.CreateDecryptor().TransformFinalBlock(cipherToDecryptBytes, 0, cipherToDecryptBytes.Length);

        return Encoding.UTF8.GetString(resultsBytes);
    }

    private static TripleDES GetCryptoServiceProvider(string key)
    {
        var provider = TripleDES.Create();

        provider.Mode = CipherMode.ECB;
        provider.Padding = PaddingMode.PKCS7;
        provider.Key = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(key));

        return provider;
    }
}
