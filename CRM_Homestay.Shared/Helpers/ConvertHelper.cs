using Newtonsoft.Json;

namespace CRM_Homestay.Core.Helpers;

public class ConvertHelper
{
    public static T ConvertDynamicTo<T>(dynamic input)
    {
        var json = JsonConvert.SerializeObject(input);
        var result = JsonConvert.DeserializeObject<T>(json);
        return result;
    }
}