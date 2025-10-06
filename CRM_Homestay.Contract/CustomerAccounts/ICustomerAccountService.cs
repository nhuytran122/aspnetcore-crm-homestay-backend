using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Users;
using CRM_Homestay.Entity.Customers;

namespace CRM_Homestay.Contract.CustomerAccounts;

public interface ICustomerAccountService
{
    Task<SignUpResponseDto> SignUpAsync(SignUpRequestDto request);
    Task LogoutAsync(Guid customerAccountId, string token);
    Task<VerifyAccountResponseDto> VerifyAsync();
    Task<BaseResponse> MapWithCustomerAsync(Guid customerAccountId, MapCustomerRequestDto request);
    Task<SignUpResponseDto> SignInAsync(SignInRequestDto request);
    Task<BaseResponse> ForgotPasswordAsync(ResetPasswordRequestDto request);
    Task<BaseResponse> ChangePasswordAsync(Guid id, ChangePasswordRequestDto request);
    Task<ProfileDto> GetProfileAsync(Guid id);
    Task<CustomerPoint> GetCustomerPointWalletAsync(Guid customerId);
    Task<ProfileRequestDto> UpdateProfileAsync(Guid id, ProfileRequestDto request);
    Task<BaseResponse> DeleteAccountAsync(Guid id);
    Task<CustomerAccount?> GetActiveAccountAsync(Guid customerAccountId);
    Task<BaseResponse> RequestDeleteAccountAsync(SignInRequestDto request);
    Task<CustomerAccount> GetCustomerAccountAsync(Guid id);
    Task ResetPasswordAsync(ResetPasswordRequestDto request, Guid id);
    Task UpdateStatusAccountAsync(Guid id, UpdateStatusDto input);
    Task<BaseResponse> RestoreDeletedAccountByAdminAsync(Guid id);
}