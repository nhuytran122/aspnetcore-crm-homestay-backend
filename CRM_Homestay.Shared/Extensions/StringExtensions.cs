using System.Globalization;
using System.Text;

namespace CRM_Homestay.Core.Extensions;

public static class StringExtensions
{
    public static string RemoveDiacritics(this string text)
    {
        return string.Concat(
            text.Normalize(NormalizationForm.FormD)
                .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
        ).Normalize(NormalizationForm.FormC);
    }

}