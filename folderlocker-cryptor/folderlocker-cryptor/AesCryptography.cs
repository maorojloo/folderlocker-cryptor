using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace folderlocker_cryptor
{
    public static class AesCryptography
    {
        /// <summary>
        /// برای رمزگذاری بایت ها استفاده می شود
        /// </summary>
        /// <param name="bytesToEncrypted">بایت های فایل یا متنی که باید رمزگذاری شود</param>
        /// <param name="passwordBytes">بایت های رشته ایی که به عنوان پسورد وارد شده است</param>
        /// <returns>آرایه ای از بایت ها</returns>
        private static byte[] AesEncrypt(byte[] bytesToEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes;
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (var memoryStream = new MemoryStream())
            {
                using (var aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);
                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(memoryStream, 
                        aes.CreateEncryptor(),
                        CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToEncrypted, 0, bytesToEncrypted.Length);
                        cs.Close();
                    }

                    encryptedBytes = memoryStream.ToArray();
                }
            }

            return encryptedBytes;
        }

        /// <summary>
        /// برای رمزگشایی بایت ها استفاده می شود
        /// </summary>
        /// <param name="bytesToDecrypted">بایت های فایل یا متنی که باید رمزگشایی شود</param>
        /// <param name="passwordBytes">بایت های رشته ایی که به عنوان پسورد وارد شده است</param>
        /// <returns>آرایه ای از بایت ها</returns>
        private static byte[] AesDecrypt(byte[] bytesToDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes;
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (var memoryStream = new MemoryStream())
            {
                using (var aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToDecrypted, 0, bytesToDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = memoryStream.ToArray();
                }
            }

            return decryptedBytes;
        }

        /// <summary>
        /// برای رمزگذاری متن استفاده می شود        
        /// </summary>
        /// <param name="input">متنی که باید رمزگذاری شود</param>
        /// <param name="password">پسوردی که متن ورودی بر اساس آن رمزگذاری می شود</param>
        /// <returns></returns>
        public static string EncryptText(string input, string password)
        {
            // بررسی آرگومان ها
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            // تبدیل متن ورودی به آرایه ای از بایت
            var bytesToEncrypted = Encoding.UTF8.GetBytes(input);
            // تبدیل پسورد به آرایه ای از بایت
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            // Hash کردن بایت های پسورد
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            // رمزگذاری بایت های متن ورودی بر اساس پسورد با استفاده از متد AesEcrypt
            var bytesEncrypted = AesEncrypt(bytesToEncrypted, passwordBytes);
            // تبدیل خروجی متد AesEcrypt به رشته 
            var result = Convert.ToBase64String(bytesEncrypted);
            // بازگردانی متن رمزگذاری شده
            return result;
        }

        /// <summary>
        /// برای رمزگشایی متن استفاده می شود     
        /// </summary>
        /// <param name="input">متن رمزگذاری شده با استفاده از متد EncryptText</param>
        /// <param name="password">پسوردی که متن ورودی بر اساس آن رمزگشایی می شود</param>
        /// <returns></returns>
        public static string DecryptText(string input, string password)
        {
            // بررسی آرگومان ها
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            // تبدیل متن ورودی به آرایه ای از بایت ها
            var bytesToDecrypted = Convert.FromBase64String(input);
            // تبدیل پسورد به آرایه ای از بایت هد
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            // تبدیل بایت های پسورد به Hash
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            // رمزگشایی بایت های متن رمزگذاری شده بر اساس پسورد با استفاده از متد AesDecrypt
            var bytesDecrypted = AesDecrypt(bytesToDecrypted, passwordBytes);
            // تبدیل خروجی متد AesDecrypt به رشته 
            var result = Encoding.UTF8.GetString(bytesDecrypted);
            // بازگردانی متن رمزگشایی شده
            return result;
        }

        /// <summary>
        /// برای رمزگذاری فایل استفاده می شود 
        /// </summary>
        /// <param name="filePath">آدرس فایلی که باید رمزگذاری شود</param>
        /// <param name="password">پسوردی که فایل باید بر اساس آن رمزگذاری می شود</param>
        public static void EncryptFile(string filePath, string password)
        {
            // بررسی آرگومان ها
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            // گرفتن کل بایت های فایل
            var bytesToEncrypted = File.ReadAllBytes(filePath);
            // تبدیل پسورد به آرایه ای از بایت ها
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            // تبدیل بایت های پسورد به Hash
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            // رمزگذاری بایت های فایل با استفاده از متد AesEncrypt
            var bytesEncrypted = AesEncrypt(bytesToEncrypted, passwordBytes);
            // نوشتن بایت ها رمزگذاری شده درون فایل
            File.WriteAllBytes(filePath, bytesEncrypted);
        }

        /// <summary>
        /// برای رمزگشایی فایل استفاده می شود
        /// </summary>
        /// <param name="filePath">آدرس فایل رمزگذاری شده با متد EncryptFile</param>
        /// <param name="password">پسوردی که فایل باید بر اساس آن رمزگشایی می شود</param>
        public static void DecryptFile(string filePath, string password)
        {
            // بررسی آرگومان ها
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            // گرفتن کل بایت های فایل
            var bytesToDecrypted = File.ReadAllBytes(filePath);
            // تبدیل پسورد به آرایه ای از بایت ها
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            // تبدیل بایت های پسورد به Hash
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            // رمزگشایی بایت های فایل با استفاده از متد AesDecrypt
            var bytesDecrypted = AesDecrypt(bytesToDecrypted, passwordBytes);
            // نوشتن بایت ها رمزگشایی شده درون فایل
            File.WriteAllBytes(filePath, bytesDecrypted);
        }
    }
}