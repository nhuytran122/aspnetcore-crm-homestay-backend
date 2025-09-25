namespace CRM_Homestay.Core.Consts.ErrorCodes;

public class BaseErrorCode
{
    public const string NotFound = "Api:0001";
    public const string ItemAlreadyExist = "Api:0002";
    public const string EmailAlreadyExist = "Api:0003";
    public const string InvalidRequirement = "Api:0004";
    public const string TryMore = "Api:0005";
    public const string InvalidTimeRange = "Api:0006";
    public const string NameOrCodeAlreadyExist = "Api:0007";
    public const string CodeAlreadyExist = "Api:0008";
    public const string NotExecute = "Api:0009";
    public const string NameAlreadyExist = "Api:0010";
    public const string PhoneNumberAlreadyExist = "Api:0011";
    public const string AccountNumberAlreadyExist = "Api:0012";
    public const string CompanyNameAlreadyExist = "Api:0014";
    public const string TaxCodeAlreadyExist = "Api:0015";
    public const string ObjectNameAlreadyExist = "Api:0016";
    public const string ObjectCodeAlreadyExist = "Api:0017";
    public const string Conflict = "Api:0018";
    public const string InvalidValue = "Api:0019";
    public const string InvalidRequest = "Api:0020";
    public const string FileNotFound = "Api:0021";
    public const string DataExist = "Api:0022";
    public const string BucketNotExist = "Api:0023";
    public const string ObjectNotFound = "Api:0024";

    public const string NotDelete = "Api:0013";
    public const string Duplicate = "Duplicate";
    public const string ZeroQuantity = "ZeroQuantity";
    public const string QuantityNotEnough = "QuantityNotEnough";
    public const string User = "User";
    public const string Branch = "Branch";
    public const string RoomType = "RoomType";
    public const string RoomPricing = "RoomPricing";
    public const string UnexpectedError = "UNEXPECTED_ERROR";
}

public class UserErrorCode
{
    public const string WrongPassword = "User:0001";
    public const string UserNameAlreadyExist = "User:0002";
    public const string LoginFailure = "User:0003";
    public const string DeactivatedUser = "User:0004";
    public const string NotFound = "User:0005";
    public const string UserRoleLimitReached = "User:0006";
    public const string InvalidDOB = "User:0007";
    public const string NotDeletedAdmin = "User:0008";
    public const string Unauthorized = "User:0009";
    public const string Forbidden = "User:0010";
    public const string ValidationFailed = "User:0011";
    public const string AccessDenied = "User:0012";
}

public class CustomerAccountErrorCode
{
    public const string PhoneNumberAlreadyExist = "PHONE_ALREADY_USED";
    public const string EmailAlreadyUsed = "EMAIL_ALREADY_USED";
    public const string InvalidGender = "INVALID_GENDER";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string OtpInvalidTokenType = "OTP_INVALID_TOKEN_TYPE";
    public const string OtpInvalidTokenCodeId = "OTP_INVALID_TOKEN_CODE_ID";
    public const string AccountNotFound = "ACCOUNT_NOT_FOUND";
    public const string UnverifiedAccount = "UNVERIFIED_ACCOUNT";
    public const string AccountBanned = "ACCOUNT_BANNED";
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
    public const string VerifyAccount = "VERIFY_ACCOUNT";
    public const string MapAccount = "MAP_ACCOUNT";
    public const string CustomerNotFound = "CUSTOMER_NOT_FOUND";
    public const string PhoneNumberMismatch = "PHONE_NUMBER_MISMATCH";
    public const string CustomerAlreadyLinked = "CUSTOMER_ALREADY_LINKED";
    public const string CustomerAccountAlreadyLinked = "CUSTOMER_ACCOUNT_ALREADY_LINKED";
    public const string Forbidden = "FORBIDDEN";
    public const string ValidationFailed = "VALIDATION_FAILED";
    public const string StatusNotAllowed = "STATUS_NOT_ALLOWED";
    public const string NotAllowUpdate = "NOT_ALLOW_UPDATE";
    public const string EmailAlreadyVerified = "EMAIL_ALREADY_VERIFIED";
    public const string AlreadyActive = "ALREADY_ACTIVE";
}

public class UploadErrorCode
{
    public const string InvalidExtension = "Upload:0001";
    public const string InvalidLength = "Upload:0002";
}

public class RoleErrorCode
{
    public const string NameAlreadyExist = "Role:0001";
}

public class MessageErrorCode
{
    public const string NotFoundChannel = "Message:0001";

}

public class DateErrorCode
{
    public const string NotMatchFormat = "DateErrorCode:0001";
    public const string QuarterIncorrect = "DateErrorCode:0002";
    public const string DateRangeIncorrect = "DateErrorCode:0003";
    public const string StartDateMustBeFuture = "DateErrorCode:0004";
}

public class BranchErrorCode
{
    public const string AlreadyExists = "Branch:0001";
    public const string NotFound = "Branch:0002";
    public const string NotAllowedDelete = "Branch:0003";
    public const string IsMainBranch = "Branch:0004";
    public const string MustHaveMainBranch = "Branch:0005";
}

public class RoomTypeErrorCode
{
    public const string AlreadyExists = "RoomType:0001";
    public const string NotFound = "RoomType:0002";
    public const string NotAllowedDelete = "RoomType:0003";
}

public class RoomPricingErrorCode
{
    public const string OverlappingDateRange = "RoomPricing:0001";
    public const string NonDefaultPricingDatesRequired = "RoomPricing:0002";
    public const string NotFound = "RoomPricing:0003";
    public const string CannotRemoveOnlyDefaultPricing = "RoomPricing:0004";
    public const string NotAllowedDelete = "RoomPricing:0005";
}