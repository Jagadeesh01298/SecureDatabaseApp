using System.Security.Cryptography;
using System.Text;

namespace SecureDatabaseApp.Services
{
    public class EncryptionService
    {
        private readonly string _encryptionKey;
        private readonly string _hmacKey;

        public EncryptionService(IConfiguration configuration)
        {
            _encryptionKey = configuration["EncryptionSettings:EncryptionKey"]!;
            _hmacKey = configuration["EncryptionSettings:HmacKey"]!;
        }

        public string Encrypt(string plainText)
        {
            byte[] key = Encoding.UTF8.GetBytes(_encryptionKey);

            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();

            using MemoryStream memoryStream = new MemoryStream();

            memoryStream.Write(aes.IV, 0, aes.IV.Length);

            using CryptoStream cryptoStream = new CryptoStream(
                memoryStream,
                aes.CreateEncryptor(),
                CryptoStreamMode.Write
            );

            using StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(plainText);
            writer.Close();

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            byte[] key = Encoding.UTF8.GetBytes(_encryptionKey);

            using Aes aes = Aes.Create();
            aes.Key = key;

            byte[] iv = new byte[aes.BlockSize / 8];
            byte[] cipherBytes = new byte[fullCipher.Length - iv.Length];

            Array.Copy(fullCipher, iv, iv.Length);
            Array.Copy(fullCipher, iv.Length, cipherBytes, 0, cipherBytes.Length);

            aes.IV = iv;

            using MemoryStream memoryStream = new MemoryStream(cipherBytes);
            using CryptoStream cryptoStream = new CryptoStream(
                memoryStream,
                aes.CreateDecryptor(),
                CryptoStreamMode.Read
            );

            using StreamReader reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }

        public string GenerateHmac(string data)
        {
            byte[] key = Encoding.UTF8.GetBytes(_hmacKey);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            using HMACSHA256 hmac = new HMACSHA256(key);
            byte[] hashBytes = hmac.ComputeHash(dataBytes);

            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyHmac(string data, string storedHmac)
        {
            string newHmac = GenerateHmac(data);
            return newHmac == storedHmac;
        }
    }
}