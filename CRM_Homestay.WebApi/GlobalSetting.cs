namespace CRM_Homestay.App;

/// <summary>
/// GlobalSetting
/// </summary>
public class GlobalSetting
{
    /// <summary>
    /// GetCultureCodes
    /// </summary>
    /// <returns></returns>
    public static List<string> GetCultureCodes()
    {
        return new List<string>()
        {
            "en-Us",
            "vi-Vi"
        };
    }
}