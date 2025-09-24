using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace CRM_Homestay.Localization;

public interface ILocalizer
{
    LocalizedString this[string name] { get; }
    LocalizedString this[string name, params object[] arguments] { get; }

}

public class Localizer : ILocalizer
{
    private readonly JsonSerializer _jsonSerializer = new JsonSerializer();
    private Dictionary<string, string> LocalizedItemsOfVn = new();
    private Dictionary<string, string> LocalizedItemsOfEn = new();

    public Localizer()
    {
        LocalizedItemsOfVn = LoadResources("vi-Vi");
        LocalizedItemsOfEn = LoadResources("en-Us");
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = GetString(name);
            return new LocalizedString(name, value ?? name, value == null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var actualValue = this[name];
            return !actualValue.ResourceNotFound
                ? new LocalizedString(name, string.Format(actualValue.Value, arguments), false)
                : actualValue;
        }
    }

    private string GetString(string name)
    {
        if (LocalizeData().ContainsKey(name))
        {
            return LocalizeData()[name];
        }

        return string.Empty;
    }

    public Dictionary<string, string> LoadResources(string code)
    {
        var LocalizedItems = new Dictionary<string, string>();
        string filePath = $@"LanguageResources/Resources/{code}.json";
        using (var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (var sReader = new StreamReader(str))
        using (var reader = new JsonTextReader(sReader))
        {
            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    continue;
                string key = reader.Value!.ToString()!;
                reader.Read();
                string value = _jsonSerializer.Deserialize<string>(reader)!;
                LocalizedItems.Add(key, value);
            }
        }

        return LocalizedItems;
    }

    private Dictionary<string, string> LocalizeData()
    {
        if (Thread.CurrentThread.CurrentCulture.Name == "en-Us")
        {
            return LocalizedItemsOfEn;
        }
        return LocalizedItemsOfVn;
    }
}