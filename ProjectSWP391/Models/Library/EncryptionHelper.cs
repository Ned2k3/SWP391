using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionHelper
{
    // Key là một chuỗi byte được sử dụng để mã hóa và giải mã dữ liệu
    // IV là một giá trị ngẫu nhiên hoặc giả ngẫu nhiên được sử dụng trong quá trình mã hóa để đảm bảo rằng cùng một plaintext, khi được mã hóa nhiều lần, sẽ tạo ra các ciphertext khác nhau.
    //gia tri key va IV 16 bit
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("5F6D7A8C9F3251E8A453B2D6C8A1F9E2");
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("2E9B8C0F3A1E6D7B");

    public static string Encrypt(string plainText)
    {
        byte[] encryptedBytes;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;

            ICryptoTransform encryptor = aes.CreateEncryptor();

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                    csEncrypt.Write(plainTextBytes, 0, plainTextBytes.Length);
                }

                encryptedBytes = msEncrypt.ToArray();
            }
        }

        return Convert.ToBase64String(encryptedBytes);
    }

    public static string Decrypt(string encryptedText)
    {
        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
        string decryptedText = null;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;

            ICryptoTransform decryptor = aes.CreateDecryptor();

            using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        decryptedText = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return decryptedText;
    }
}