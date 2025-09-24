using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Claims;
using CRM_Homestay.Contract.Users;
using CRM_Homestay.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Homestay.App.Controllers;

/// <summary>
/// UserController
/// </summary>
[ApiController]
[Route("api/user/")]
[Authorize]
public class UserController : BaseController
{
    private IUserService _userService;
    /// <summary>
    /// UserController init
    /// </summary>
    /// <param name="userService"></param>
    /// <param name="httpContextAccessor"></param>
    public UserController(IUserService userService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        _userService = userService;
    }

    /// <summary>
    ///   Get thông tin profile cá nhân
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("get-personal-profile")]
    public async Task<UserDto> GetPersonalProfileAsync()
    {
        return await _userService.GetAsync(GetCurrentUserId());
    }

    /// <summary>
    ///   cập nhật thông tin profile cá nhân
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    [Route("update-personal-profile")]
    public async Task<UserDto> UpdatePersonalProfileAsync(UpdateProfileRequestDto input)
    {
        return await _userService.UpdateProfilesAsync(input, GetCurrentUserId());
    }

    /// <summary>
    ///   xóa avatar cá nhân
    /// </summary>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete-personal-avatar")]
    public async Task DeleteAvatar()
    {
        await _userService.DeleteAvatarAsync(GetCurrentUserId());
    }

    /// <summary>
    ///  đăng xuất 
    /// </summary>
    [HttpPut]
    [Route("personal-logout")]
    public async Task Logout()
    {
        await _userService.Logout(GetCurrentUserId());
    }

    /// <summary>
    ///   Xóa User
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete]
    [Route("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _userService.DeleteAsync(id);
    }

    /// <summary>
    ///  set 1 password mới
    /// </summary>
    /// <param name="input"></param>
    [HttpPut]
    [Route("set-new-password")]
    public async Task SetNewPasswordAsync(NewPasswordRequestDto input)
    {
        await _userService.SetNewPasswordAsync(input, GetCurrentUserId());
    }

    /// <summary>
    ///  Get quyền cá nhân của user (ví dụ user đó có được tạo,cập nhật xóa trên hệ thống không)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("get-claims/{id}")]
    public async Task<List<ClaimDto>> GetClaims(Guid id)
    {
        return await _userService.GetClaims(id);
    }

    /// <summary>
    ///  Get quyền từ user của role (ví dụ user đó có được tạo,cập nhật xóa trên hệ thống không)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("get-user-claims-and-role-claims/{id}")]
    public async Task<UserClaimsDto> GetUserClaimsAndRoleClaims(Guid id)
    {
        return await _userService.GetUserClaimsAndRoleClaims(id);
    }

    /// <summary>
    ///  cập nhật quyền của user
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    [HttpPut]
    [Route("update-user-claims/{id}")]
    public async Task UpdateUserClaims(Guid id, List<CreateUpdateClaimDto> input)
    {
        await _userService.UpdateUserClaims(id, input);
    }

    /// <summary>
    ///  get danh sách user cùng với chi tiết 
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("get-list-with-navigation-properties")]
    public async Task<PagedResultDto<UserWithNavigationPropertiesDto>> GetListWithNavigationAsync([FromQuery] UserFilterDto filter)
    {
        return await _userService.GetListWithNavigationPropertiesAsync(filter);
    }

    /// <summary>
    ///  get user cùng với chi tiết
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("get-with-navigation-properties/{id}")]
    [Authorize(Roles = $"{nameof(RoleEnum.admin)}, {nameof(RoleEnum.technical_staff)}")]
    public async Task<UserWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
    {
        return await _userService.GetWithNavigationPropertiesAsync(id);
    }

    /// <summary>
    ///  tạo user cùng với chi tiết của nó
    /// </summary>
    /// <param name="input"></param>
    [HttpPost]
    [Authorize(Roles = $"{nameof(RoleEnum.admin)}, {nameof(RoleEnum.technical_staff)}")]
    [Route("create-with-navigation-properties")]
    public async Task CreateWithNavigationPropertiesAsync(CreateUserDto input)
    {
        await _userService.CreateWithNavigationPropertiesAsync(input, GetCurrentRole());
    }

    /// <summary>
    ///  cập nhật user cùng với chi tiết của nó
    /// </summary>
    /// <param name="input"></param>
    /// <param name="id"></param>
    [HttpPut]
    [Route("update-with-navigation-properties/{id}")]
    [Authorize(Roles = $"{nameof(RoleEnum.admin)}, {nameof(RoleEnum.technical_staff)}")]
    public async Task UpdateWithNavigationPropertiesAsync(UpdateUserDto input, Guid id)
    {
        await _userService.UpdateWithNavigationPropertiesAsync(input, id);
    }

    /// <summary>
    ///  đăng nhập
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("sign-in")]
    [AllowAnonymous]
    public async Task<UserAccessInfoDto> SignInAsync(LoginRequestDto request)
    {
        return await _userService.SignInAsync(request);
    }

    /// <summary>
    /// reset lại password
    /// </summary>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("reset-password/{id}")]
    [Authorize(Roles = $"{nameof(RoleEnum.admin)}, {nameof(RoleEnum.technical_staff)}")]
    public async Task ResetPasswordAsync(ResetRequestDto request, Guid id)
    {
        await _userService.ResetPasswordAsync(request, id);
    }

    /// <summary>
    /// Lấy toàn bộ nhân viên
    /// </summary>
    /// <param name="isAll"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<BasicUserDto>> GetAllUser([FromQuery] bool isAll)
    {
        return await _userService.GetAllUserAsync(isAll);
    }
}