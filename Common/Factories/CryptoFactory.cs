using System;
using System.Security.Cryptography;
using System.Text;

namespace Common.Factories
{
    public static class CryptoFactory
    {
        public static string Encrypt(string key, string dataToEncrypt)
        {
            var cryptoServiceProvider = GetCryptoServiceProvider(key);
            var dataToEncryptBytes = UTF8Encoding.UTF8.GetBytes(dataToEncrypt);

            var resultsBytes = cryptoServiceProvider.CreateEncryptor().TransformFinalBlock(dataToEncryptBytes, 0, dataToEncryptBytes.Length);

            return Convert.ToBase64String(resultsBytes, 0, resultsBytes.Length);
        }

        public static string Decrypt(string key, string cipherToDecrypt)
        {
            var cryptoServiceProvider = GetCryptoServiceProvider(key);
            var cipherToDecryptBytes = Convert.FromBase64String(cipherToDecrypt);

            var resultsBytes = cryptoServiceProvider.CreateDecryptor().TransformFinalBlock(cipherToDecryptBytes, 0, cipherToDecryptBytes.Length);

            return UTF8Encoding.UTF8.GetString(resultsBytes);
        }

        private static TripleDESCryptoServiceProvider GetCryptoServiceProvider(string key)
        {
            return new TripleDESCryptoServiceProvider
            {
                Key = new MD5CryptoServiceProvider().ComputeHash(UTF8Encoding.UTF8.GetBytes(key)),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
        }
    }
}