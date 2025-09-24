using Newtonsoft.Json;

namespace CRM_Homestay.Core.Extensions;

public static class ObjectExtensions
{
    public static T Clone<T>(this T source)
    {
        var serialized = JsonConvert.SerializeObject(source);
        return JsonConvert.DeserializeObject<T>(serialized)!;
    }

    public static List<T> CloneList<T>(this IEnumerable<T> sources)
    {
        var cloneList =
            new List<T>();
        foreach (var item in sources)
        {
            cloneList.Add(item.Clone());
        }

        return cloneList;
    }



}