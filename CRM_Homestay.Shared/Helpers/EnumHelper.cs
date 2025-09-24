using CRM_Homestay.Core.Extensions;
using System.ComponentModel;

namespace CRM_Homestay.Core.Helpers
{
    public static class EnumHelper
    {
        public static TEnum ParseEnumFromDescription<TEnum>(string description)
        {
            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
            {
                var enumMember = typeof(TEnum).GetField(enumValue.ToString()!);
                var attribute = (DescriptionAttribute)enumMember!.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault()!;

                if (attribute != null && attribute.Description == description)
                {
                    return enumValue;
                }
            }

            throw new ArgumentException($"No enum value found for description: {description}");
        }

        public static List<EnumModels> ToEnumLists<T>()
        {
            var enumModels = new List<EnumModels>();
            var t = typeof(T);
            if (!t.IsEnum) return enumModels;

            var values = Enum.GetValues(t).Cast<Enum>().Select(e => new
            {
                Value = e.GetHashCode(),
                Name = e.GetDescription()
            });

            enumModels.AddRange(values.Select(item => new EnumModels { Value = item.Value, Name = item.Name }));
            return enumModels;
        }

        public class EnumModels
        {
            /// <summary>
            /// Code
            /// </summary>
            public int? Value { get; set; }

            /// <summary>
            /// Name enum
            /// </summary>
            public string? Name { get; set; }
            public int? TypeCategory
            {
                get { return (Value == 1 || Value == 2) ? 0 : 1; }
            }
        }
    }
}
