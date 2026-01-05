using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Covenant.Test.Utils
{
    public static class FileAssert
    {
        private static string GetFileHash(string filename)
        {
            Assert.True(File.Exists(filename));

            using (var hash = SHA1.Create())
            {
                var clearBytes = File.ReadAllBytes(filename);
                var hashedBytes = hash.ComputeHash(clearBytes);
                return ConvertBytesToHex(hashedBytes);
            }
        }

        private static string ConvertBytesToHex(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte t in bytes) sb.Append(t.ToString("x"));
            return sb.ToString();
        }

        public static void Equal(string filename1, string filename2)
        {
            string hash1 = GetFileHash(filename1);
            string hash2 = GetFileHash(filename2);

            Assert.Equal(hash1, hash2);
        }
    }
}