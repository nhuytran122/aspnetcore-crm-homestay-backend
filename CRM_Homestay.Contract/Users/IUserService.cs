using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Claims;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.Users;

public interface IUserService : IApplicationService
{
    public Task<BasicUserDto> GetBasicUserAsync(Guid id);
    public Task<PagedResultDto<UserWithNavigationPropertiesDto>> GetListWithNavigationPropertiesAsync(UserFilterDto filter);


    public Task<UserWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id, Guid currentId, string role);
    public Task<UserDto> GetAsync(Guid id);

    public Task CreateWithNavigationPropertiesAsync(CreateUserDto input, string role);
    public Task UpdateWithNavigationPropertiesAsync(UpdateUserDto input, Guid id, Guid currentId, string role);
    public Task<UserDto> UpdateProfilesAsync(UpdateProfileRequestDto input, Guid id);
    public Task<List<UserDto>> GetListAsync();

    public Task<UserAccessInfoDto> SignInAsync(LoginRequestDto request);
    public Task<UserDto> SignUpAsync(CreateUserDto input);


    public Task ResetPasswordAsync(ResetRequestDto request, Guid id, Guid currentId, string role);
    public Task SetNewPasswordAsync(NewPasswordRequestDto input, Guid id);

    public Task<List<ClaimDto>> GetClaims(Guid id);
    public Task<UserClaimsDto> GetUserClaimsAndRoleClaims(Guid id);

    public Task UpdateUserClaims(Guid id, List<CreateUpdateClaimDto> input);


    public Task<UserDto> CreateAsync(CreateUserDto input, bool save = true);
    public Task<UserDto> UpdateAsync(UpdateUserDto input, Guid id, bool save = true);
    public Task DeleteAsync(Guid id);
    public Task DeleteAvatarAsync(Guid id);

    public Task Logout(Guid id);

    Task<List<BasicUserDto>> GetAllUserAsync(bool isAll);


}