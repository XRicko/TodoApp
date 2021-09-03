using System;
using System.Security.Cryptography;

namespace ToDoList.Core.Services
{
    internal class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;

        public string Hash(string password, int iterations = 10000)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException($"'{nameof(password)}' cannot be null or whitespace", nameof(password));

            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            byte[] hashBytes = new byte[SaltSize + HashSize];

            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            string base64Hash = Convert.ToBase64String(hashBytes);

            return string.Format("$MYHASH$V1${0}${1}", iterations, base64Hash);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException($"'{nameof(password)}' cannot be null or whitespace", nameof(password));
            if (string.IsNullOrWhiteSpace(hashedPassword))
                throw new ArgumentException($"'{nameof(hashedPassword)}' cannot be null or whitespace", nameof(hashedPassword));

            if (!IsHashSupported(hashedPassword))
                throw new NotSupportedException("The hashtype is not supported");

            string[] splittedHashString = hashedPassword.Replace("$MYHASH$V1$", "").Split('$');

            int iterations = int.Parse(splittedHashString[0]);
            string base64Hash = splittedHashString[1];

            byte[] hashBytes = Convert.FromBase64String(base64Hash);
            byte[] salt = new byte[SaltSize];

            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            for (int i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                    return false;
            }

            return true;
        }

        private static bool IsHashSupported(string hashString) => hashString.Contains("$MYHASH$V1$");
    }
}
