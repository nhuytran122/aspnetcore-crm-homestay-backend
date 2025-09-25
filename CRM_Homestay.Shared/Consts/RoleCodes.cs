namespace CRM_Homestay.Core.Consts;

public static class RoleCodes
{
    public const string ADMIN = "admin";
    public const string RECEPTIONIST = "receptionist";
    public const string TECHNICAL_STAFF = "technical_staff";
    public const string HR_STAFF = "hr_staff";
    public const string HOUSEKEEPING_STAFF = "housekeeping_staff";
    public const string SERVICE_STAFF = "service_staff";

    public const string ALL = $"{ADMIN},{TECHNICAL_STAFF},{RECEPTIONIST},{HR_STAFF},{HOUSEKEEPING_STAFF},{SERVICE_STAFF}";
    public const string ADMIN_AND_TECHNICAL = $"{ADMIN},{TECHNICAL_STAFF}";
}
