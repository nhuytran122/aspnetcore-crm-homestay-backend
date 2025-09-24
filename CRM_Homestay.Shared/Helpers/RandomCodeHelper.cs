using System;

namespace CRM_Homestay.Core.Helpers;

public static class RandomCodeHelper
{
    public static string GenerateRandomCode(int startRange, int startEnd, int codeCount)
    {
        Random rnd = new Random();
        var code = "";

        for (int i = 0; i < codeCount; i++)
        {
            code += rnd.Next(startRange, startEnd).ToString();
        }
        return code;
    }

    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string GenerateReferralCode(int length = 12)
    {
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static string GenerateRandomCode(int length = 5)
    {
        var random = new Random();
        return new string(Enumerable.Range(0, length)
            .Select(c => chars[random.Next(chars.Length)])
            .ToArray());
    }
}