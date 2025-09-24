using System.Security.Claims;

namespace CRM_Homestay.Core.Consts.Permissions;

public class AccessClaims
{
    public const string GroupName = "CRM_Homestay";

    public static class Users
    {
        public const string Default = GroupName + ".Users";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Authorize = Default + ".Authorize";
    }

    public static class Roles
    {
        public const string Default = GroupName + ".Roles";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Authorize = Default + ".Authorize";
    }

}

public class ExtendClaimTypes
{
    public const string Permission = "permission";
    public const string Tenant = "tenant";
}