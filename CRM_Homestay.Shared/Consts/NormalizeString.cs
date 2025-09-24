using System.Text;
using System.Text.RegularExpressions;

namespace CRM_Homestay.Core.Consts
{
    public static class NormalizeString
    {
        public static string ConvertNormalizeString(string input)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = input.Trim().Normalize(NormalizationForm.FormD);
            return (regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').ToUpper()).Trim();
        }
    }
}
