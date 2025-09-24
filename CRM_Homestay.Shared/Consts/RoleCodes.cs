namespace CRM_Homestay.Core.Consts;

public static class RoleCodes
{
    public const string ADMIN = "admin";
    public const string OFFICE_STAFF = "office_staff";
    public const string TECHNICAL_STAFF = "technical_staff";
    public const string HR_STAFF = "hr_staff";
    public const string ALL = $"{ADMIN},{TECHNICAL_STAFF},{OFFICE_STAFF},{HR_STAFF}";
}
