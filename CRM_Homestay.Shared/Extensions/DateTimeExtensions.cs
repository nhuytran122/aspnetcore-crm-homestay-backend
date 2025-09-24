namespace CRM_Homestay.Core.Extensions;

public static class DateTimeExtensions
{
    public static string ToDateVn(this DateTime datetime)
    {
        return $"{datetime:dd-MM-yyyy}";
    }
}