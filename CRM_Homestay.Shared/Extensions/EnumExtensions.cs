using System.ComponentModel;
using System.Reflection;

namespace CRM_Homestay.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString())!;
            DescriptionAttribute? attribute = (DescriptionAttribute?)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute), false);

            return attribute != null ? attribute.Description : value.ToString();

        }

        public static TEnum GetValueFromDescription<TEnum>(this string description) where TEnum : Enum
        {
            var enumType = typeof(TEnum);

            foreach (var field in enumType.GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                    {
                        return (TEnum)field.GetValue(null)!;
                    }
                }
            }

            throw new ArgumentException($"No enum value with description '{description}' found.");
        }
    }
}
