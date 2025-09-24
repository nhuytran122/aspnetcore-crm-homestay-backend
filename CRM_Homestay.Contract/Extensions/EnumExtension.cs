using System.ComponentModel;
namespace CRM_Homestay.Contract.Extensions;

public static class EnumExtension
{
    public static string GetDescriptionOrName<T>(this T enumValue) where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
        {
            return string.Empty;
        }

        var result = enumValue.ToString();
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString() ?? string.Empty);

        if (fieldInfo != null)
        {
            var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs.Length > 0)
            {
                result = ((DescriptionAttribute)attrs[0]).Description;
            }
        }
        else
        {
            result = System.Enum.GetName(typeof(T), enumValue);
        }

        return result ?? string.Empty;
    }
}