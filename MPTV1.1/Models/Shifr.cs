using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace College_API_V1.Models
{
    public class Shifr
    {
        private readonly string privateKey;
        private readonly string publicKey;

        public Shifr()
        {
            // Инициализация ключей при создании экземпляра класса
            string publicKeyPath = "C:\\Users\\Telepuzik\\source\\repos\\MPT_API_V1.2\\publicKey.txt";
            string privateKeyPath = "C:\\Users\\Telepuzik\\source\\repos\\MPT_API_V1.2\\privateKey.txt";
            publicKey = File.ReadAllText(publicKeyPath);
            privateKey = File.ReadAllText(privateKeyPath);
        }

        public async Task<string> EncryptDataAsync(string originalData)
        {
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(originalData);
            byte[] encryptedData = await EncryptAsync(dataToEncrypt, publicKey);
            return Convert.ToBase64String(encryptedData);
        }

        public async Task<string> DecryptDataAsync(string encryptedData)
        {
            byte[] dataToDecrypt = Convert.FromBase64String(encryptedData);
            return await DecryptAsync(dataToDecrypt, privateKey);
        }

        private static async Task<byte[]> EncryptAsync(byte[] data, string publicKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                return await Task.Run(() => rsa.Encrypt(data, false));
            }
        }

        private static async Task<string> DecryptAsync(byte[] data, string privateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                return await Task.Run(() => Encoding.UTF8.GetString(rsa.Decrypt(data, false)));
            }
        }
    }
}
