namespace CRM_Homestay.Shared.Helpers;

using System;
using System.Security.Cryptography;
using System.Text;

public static class PkceHelper
{
    public static string GenerateCodeVerifier(int length = 43)
    {
        if (length < 43 || length > 128) throw new ArgumentOutOfRangeException(nameof(length));
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            int idx = RandomNumberGenerator.GetInt32(0, chars.Length);
            sb.Append(chars[idx]);
        }
        return sb.ToString();
    }

    // Tạo code_challenge = BASE64URL(SHA256(ASCII(code_verifier)))
    public static string CreateCodeChallenge(string codeVerifier)
    {
        // SHA256 trên bytes ASCII
        var bytes = Encoding.ASCII.GetBytes(codeVerifier);
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(bytes);
        return Base64UrlEncode(hash);
    }

    // Base64 URL-safe (replace +/, remove padding)
    private static string Base64UrlEncode(byte[] input)
    {
        var s = Convert.ToBase64String(input)
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .TrimEnd('=');
        return s;
    }
}
