using System.Security.Cryptography;
using System.Text;

namespace CRM_Homestay.Core.Helpers
{
    public static class HashHelper
    {
        public static string Sha256(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha.ComputeHash(bytes);

                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        public static bool VerifySha256(string input, string hash)
        {
            var inputHash = Sha256(input);
            return string.Equals(inputHash, hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
