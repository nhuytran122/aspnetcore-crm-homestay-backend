using AutoMapper;
using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Claims;
using CRM_Homestay.Contract.Locations;
using CRM_Homestay.Contract.Password;
using CRM_Homestay.Contract.Roles;
using CRM_Homestay.Contract.Users;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Helper;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.Roles;
using CRM_Homestay.Entity.UserClaims;
using CRM_Homestay.Entity.UserRoles;
using CRM_Homestay.Entity.Users;
using CRM_Homestay.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CRM_Homestay.Service.Users;

public class UserService : BaseService, IUserService
{
    private IConfiguration _configuration;
    private UserManager<User> _userManager { get; set; }
    private RoleManager<Role> _rolerManager { get; set; }

    private ILocationServiceShared _locationServiceShared;

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(ILocationServiceShared locationServiceShared, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l, UserManager<User> userManager, RoleManager<Role> rolerManager,
        IConfiguration configuration, IPasswordHasher passwordHasher) : base(unitOfWork, mapper, l)
    {
        _userManager = userManager;
        _configuration = configuration;
        _rolerManager = rolerManager;
        _locationServiceShared = locationServiceShared;
        _httpContextAccessor = httpContextAccessor;
        _passwordHasher = passwordHasher;
    }

    public async Task<BasicUserDto> GetBasicUserAsync(Guid id)
    {
        var item = await _unitOfWork.GenericRepository<User>().GetQueryable()
            .Where(x => x.Id == id)
            .Select(x => new BasicUserDto()
            {
                Id = x.Id,
                FullName = $"{x.FirstName} {x.LastName}",
                UserName = x.UserName!,
                FirstName = x.FirstName!,
                LastName = x.LastName!,
            }).FirstOrDefaultAsync();
        if (item == null) throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.NotFound);

        return item;
    }

    public async Task<PagedResultDto<UserWithNavigationPropertiesDto>> GetListWithNavigationPropertiesAsync(UserFilterDto filter)
    {
        if (!filter.IsValidFilter())
        {
            throw new GlobalException(L[BaseErrorCode.InvalidRequirement], HttpStatusCode.BadRequest);
        }
        var result = await _unitOfWork.CommonQueries
            .QueryUserListWithNavigationProperties(filter.StartDate, filter.EndDate, filter.Text
                                                           , filter.Gender, filter.RoleId, filter.IsActive)
            .GetPaged(filter.CurrentPage, filter.PageSize);

        return ObjectMapper
            .Map<PagedResult<UserWithNavigationProperties>, PagedResultDto<UserWithNavigationPropertiesDto>>(result);
    }

    public async Task<UserWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
    {
        // TODO Get cashTransaction Info

        // Access the TotalAmountHeld field

        return ObjectMapper.Map<UserWithNavigationProperties, UserWithNavigationPropertiesDto>(
        await _unitOfWork.CommonQueries.QueryUserWithNavigationProperties(id)
        .Select(userWithNavProps => new UserWithNavigationProperties
        {
            User = userWithNavProps.User,
            Role = userWithNavProps.Role,
        })
        .FirstOrDefaultAsync() ?? new UserWithNavigationProperties());
    }

    public async Task<UserDto> GetAsync(Guid id)
    {
        var user = await _unitOfWork.GenericRepository<User>().GetQueryable().AsNoTracking()
                                    .Include(x => x.UserRoles!)
                                        .ThenInclude(ur => ur.Role)
                                    .FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.NotFound);

        var result = ObjectMapper.Map<User, UserDto>(user);
        result.RoleName = user.UserRoles!.First().Role!.Description;
        return result;
    }

    public async Task CreateWithNavigationPropertiesAsync(CreateUserDto input, string currentRole)
    {

        if (currentRole == RoleCodes.TECHNICAL_STAFF)
        {
            var createRole = await _unitOfWork.GenericRepository<Role>().GetQueryable().FirstOrDefaultAsync(x => x.Id == input.RoleId);

            if (createRole?.Name == RoleCodes.ADMIN || createRole?.Name == RoleCodes.TECHNICAL_STAFF)
            {
                throw new GlobalException(L[UserErrorCode.AccessDenied], HttpStatusCode.Forbidden);
            }
        }

        var newUser = await CreateAsync(input, false);
        if (input.RoleId != Guid.Empty)
        {
            await DefineRoleToNewUser(newUser.Id, input.RoleId, false);
        }
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task UpdateWithNavigationPropertiesAsync(UpdateUserDto input, Guid id)
    {
        var updatedUser = await UpdateAsync(input, id, true);

        var checkRoles = await _unitOfWork.GenericRepository<UserRole>().GetListAsync(x => x.UserId == id && x.RoleId == input.RoleId);
        if (input.RoleId != Guid.Empty && checkRoles.Count <= 0)
        {
            await ClearAndDefineRoleToUser(updatedUser.Id, input.RoleId, false);

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null && user.IsLoggedIn)
            {
                user.IsLoggedIn = false;
                _unitOfWork.GenericRepository<User>().Update(user);
            }
        }
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task<UserDto> UpdateProfilesAsync(UpdateProfileRequestDto input, Guid id)
    {
        HandleInput(input);
        var userRepo = _unitOfWork.GenericRepository<User>();
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
        {
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.BadRequest);
        }

        user = ObjectMapper.Map(input, user);

        if (!input.Email.IsNullOrWhiteSpace())
        {
            if (await userRepo.AnyAsync(x => x.NormalizedEmail == input.Email.ToUpper() && x.Id != id))
            {
                throw new GlobalException(L[BaseErrorCode.EmailAlreadyExist], HttpStatusCode.BadRequest);
            }
        }

        if (await userRepo.AnyAsync(x => x.PhoneNumber == input.PhoneNumber && x.Id != id))
        {
            throw new GlobalException(L[BaseErrorCode.PhoneNumberAlreadyExist], HttpStatusCode.BadRequest);
        }
        var inputAddress = input.Address;

        var location = await _locationServiceShared.GetLocations(new LocationRequestDto()
        {
            ProvinceId = inputAddress.ProvinceId,
            DistrictId = inputAddress.DistrictId,
            WardId = inputAddress.WardId,
        });

        var joinedAddress = $"{inputAddress.Street}, {location.Ward}, {location.District}, {location.Province}";
        user.Address!.JoinedName = joinedAddress;
        user.Address.WardName = location.Ward;
        user.Address.DistrictName = location.District;
        user.Address.ProvinceName = location.Province;
        user.NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{user.FirstName} {user.LastName}");
        user.NormalizeAddress = NormalizeString.ConvertNormalizeString(joinedAddress.Replace(",", ""));

        _unitOfWork.GenericRepository<User>().Update(user);
        await _unitOfWork.SaveChangeAsync();

        return ObjectMapper.Map<User, UserDto>(user);
    }

    public async Task<List<UserDto>> GetListAsync()
    {
        var users = await _userManager.Users.ToListAsync();

        return ObjectMapper.Map<List<User>, List<UserDto>>(users);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto input, bool save = true)
    {
        HandleInput(input);
        var userRepo = _unitOfWork.GenericRepository<User>();

        var user = ObjectMapper.Map<CreateUserDto, User>(input);
        var inputAddress = input.Address;
        var location = await _locationServiceShared.GetLocations(new LocationRequestDto()
        {
            ProvinceId = inputAddress.ProvinceId,
            DistrictId = inputAddress.DistrictId,
            WardId = inputAddress.WardId,
        });

        var joinedAddress = $"{inputAddress.Street}, {location.Ward}, {location.District}, {location.Province}";

        if (await userRepo
                .GetQueryable().IgnoreQueryFilters()
                .AnyAsync(x => x.NormalizedUserName == user.UserName!.ToUpper()))
        {
            throw new GlobalException(L[UserErrorCode.UserNameAlreadyExist], HttpStatusCode.BadRequest);
        }

        if (!input.Email.IsNullOrWhiteSpace())
        {
            if (await userRepo.AnyAsync(x => x.NormalizedEmail == input.Email.ToUpper()))
            {
                throw new GlobalException(L[BaseErrorCode.EmailAlreadyExist], HttpStatusCode.BadRequest);
            }
        }

        user.Address!.JoinedName = joinedAddress;
        user.Address.WardName = location.Ward;
        user.Address.DistrictName = location.District;
        user.Address.ProvinceName = location.Province;
        user.NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{user.FirstName} {user.LastName}");
        user.NormalizeAddress = NormalizeString.ConvertNormalizeString(joinedAddress.Replace(",", ""));

        user.SecurityStamp = Guid.NewGuid().ToString();
        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(new User(), input.Password!);
        await userRepo.AddAsync(user);

        if (save)
        {
            await _unitOfWork.SaveChangeAsync();
        }
        return ObjectMapper.Map<User, UserDto>(user);
    }

    public async Task<UserDto> UpdateAsync(UpdateUserDto input, Guid id, bool save = true)
    {
        HandleInput(input);
        var item = await _userManager.FindByIdAsync(id.ToString());
        if (item == null)
        {
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.BadRequest);
        }
        var user = ObjectMapper.Map(input, item);
        var inputAddress = input.Address;
        var location = await _locationServiceShared.GetLocations(new LocationRequestDto()
        {
            ProvinceId = inputAddress!.ProvinceId,
            DistrictId = inputAddress.DistrictId,
            WardId = inputAddress.WardId,
        });

        var joinedAddress = $"{inputAddress.Street}, {location.Ward}, {location.District}, {location.Province}";

        var userRepo = _unitOfWork.GenericRepository<User>();
        if (!input.Email.IsNullOrWhiteSpace())
        {
            if (await userRepo.AnyAsync(x => x.NormalizedEmail == input.Email.ToUpper() && x.Id != id))
            {
                throw new GlobalException(L[BaseErrorCode.EmailAlreadyExist], HttpStatusCode.BadRequest);
            }
        }
        if (await userRepo.AnyAsync(x => x.PhoneNumber == input.PhoneNumber && x.Id != id))
        {
            throw new GlobalException(L[BaseErrorCode.PhoneNumberAlreadyExist], HttpStatusCode.BadRequest);
        }

        user.Address!.JoinedName = joinedAddress;
        user.Address.WardName = location.Ward;
        user.Address.DistrictName = location.District;
        user.Address.ProvinceName = location.Province;
        user.NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{user.FirstName} {user.LastName}");
        user.NormalizeAddress = NormalizeString.ConvertNormalizeString(joinedAddress.Replace(",", ""));

        _unitOfWork.GenericRepository<User>().Update(user);
        if (save)
        {
            await _unitOfWork.SaveChangeAsync();
        }
        return ObjectMapper.Map<User, UserDto>(user);
    }

    public async Task DeleteAsync(Guid id)
    {
        var item = await _unitOfWork.GenericRepository<User>().GetQueryable()
            .Where(x => x.Id == id && !x.DeletedAt.HasValue)
            //TODO: Xử lý giao dịch liên quan
            //.Include(x => x.CashTransactions)
            .FirstOrDefaultAsync();
        if (item == null)
        {
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.BadRequest);
        }
        if (await _userManager.IsInRoleAsync(item, RoleCodes.ADMIN))
        {
            throw new GlobalException(L[UserErrorCode.NotDeletedAdmin], HttpStatusCode.BadRequest);
        }
        item.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _unitOfWork.GenericRepository<User>().Update(item);
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task DeleteAvatarAsync(Guid id)
    {
        var item = await _userManager.FindByIdAsync(id.ToString());
        if (item == null)
        {
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.BadRequest);
        }
        item.AvatarURL = null;
        _unitOfWork.GenericRepository<User>().Update(item);
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task Logout(Guid id)
    {
        var userRepo = _unitOfWork.GenericRepository<User>();
        var user = await userRepo.GetQueryable().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
        {
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.BadRequest);
        }
        user.IsLoggedIn = false;
        userRepo.Update(user);
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task<UserAccessInfoDto> SignInAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName!);
        if (user == null || user.DeletedAt.HasValue)
        {
            throw new GlobalException(L[UserErrorCode.NotFound], HttpStatusCode.BadRequest);
        }
        if (!user.IsActive)
        {
            throw new GlobalException(L[UserErrorCode.DeactivatedUser], HttpStatusCode.BadRequest);
        }
        var result = await _userManager.CheckPasswordAsync(user, request.Password!);
        if (!result)
        {
            throw new GlobalException(L[UserErrorCode.WrongPassword], HttpStatusCode.BadRequest);
        }
        var userAccessInfoDto = await GenerateTokenWithInfoByUser(user);
        user.IsLoggedIn = true;
        _unitOfWork.GenericRepository<User>().Update(user);

        await _unitOfWork.SaveChangeAsync();
        return userAccessInfoDto;
    }

    public Task<UserDto> SignUpAsync(CreateUserDto input)
    {
        return CreateAsync(input);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequestDto request, Guid id)
    {

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.BadRequest);
        }

        if (user.SecurityStamp == null)
        {
            await _userManager.UpdateSecurityStampAsync(user);
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword!);
        if (!result.Succeeded)
        {
            throw new GlobalException(L[BaseErrorCode.TryMore], HttpStatusCode.BadRequest);
        }
    }

    public async Task SetNewPasswordAsync(ChangePasswordRequestDto input, Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.BadRequest);
        }

        if (!await _userManager.CheckPasswordAsync(user, input.OldPassword!))
        {
            throw new GlobalException(L[UserErrorCode.WrongPassword], HttpStatusCode.BadRequest);
        }

        if (user.SecurityStamp == null)
        {
            await _userManager.UpdateSecurityStampAsync(user);
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, input.NewPassword!);
        if (!result.Succeeded)
        {
            throw new GlobalException(result.Errors.FirstOrDefault()!.Description, HttpStatusCode.BadRequest);
        }
    }
    public async Task<List<ClaimDto>> GetClaims(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.BadRequest);
        }

        var claims = (List<Claim>)await _userManager.GetClaimsAsync(user);

        return ObjectMapper.Map<List<Claim>, List<ClaimDto>>(claims);
    }

    public async Task<UserClaimsDto> GetUserClaimsAndRoleClaims(Guid id)
    {
        var userClaims = await GetClaims(id);
        var userRoleIds = await _unitOfWork.CommonQueries.QueryRoleIds(id).ToListAsync();
        var roleClaims = await _unitOfWork.CommonQueries.QueryClaimsByRoleIds(userRoleIds).ToListAsync();

        return new UserClaimsDto()
        {
            RoleClaims = ObjectMapper.Map<List<Claim>, List<ClaimDto>>(roleClaims),
            UserClaims = userClaims
        };
    }

    public async Task UpdateUserClaims(Guid id, List<CreateUpdateClaimDto> input)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.BadRequest);
        }

        var oldUserClaims = _unitOfWork
            .GenericRepository<UserClaim>().GetList(x => x.UserId == id)
            .Select(x => new Claim(x.ClaimType!, x.ClaimValue!));

        var newClaims = input.Select(x => new Claim(x.ClaimType!, x.ClaimValue!));
        await _userManager.RemoveClaimsAsync(user, oldUserClaims);
        await _userManager.AddClaimsAsync(user, newClaims);
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task DefineRoleToNewUser(Guid userId, Guid roleId, bool save = true)
    {
        if (!await _unitOfWork.GenericRepository<Role>().AnyAsync(x => x.Id == roleId))
        {
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.BadRequest);
        }
        await _unitOfWork.GenericRepository<UserRole>().AddAsync(new UserRole()
        {
            RoleId = roleId,
            UserId = userId
        });

        if (save)
        {
            await _unitOfWork.SaveChangeAsync();
        }
    }

    public async Task ClearAndDefineRoleToUser(Guid userId, Guid roleId, bool save = true)
    {
        if (!await _unitOfWork.GenericRepository<Role>().AnyAsync(x => x.Id == roleId))
        {
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.BadRequest);
        }
        var user = await _userManager.FindByIdAsync(userId.ToString());

        var oldRoles = await _unitOfWork.GenericRepository<UserRole>().GetListAsync(x => x.UserId == userId);
        _unitOfWork.GenericRepository<UserRole>().RemoveRange(oldRoles);

        await _unitOfWork.GenericRepository<UserRole>().AddAsync(new UserRole()
        {
            RoleId = roleId,
            UserId = userId
        });
        if (save)
        {
            await _unitOfWork.SaveChangeAsync();
        }
    }

    private async Task<UserAccessInfoDto> GenerateTokenWithInfoByUser(User user)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        List<Claim> claims = new List<Claim>();
        var userRoles = await _unitOfWork
            .CommonQueries.QueryRoles(user.Id, true).ToListAsync();
        foreach (var item in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, item.Name!));
        }

        var roleClaims = await _unitOfWork
            .CommonQueries.QueryClaimsByRoleIds(userRoles.Select(x => x.Id).ToList()).ToListAsync();
        var userClaims = _unitOfWork.GenericRepository<UserClaim>()
            .GetListAsync(x => x.UserId == user.Id)
            .Result
            .Select(x => new Claim(x.ClaimType!, x.ClaimValue!));
        claims.AddRange(roleClaims);
        claims.AddRange(userClaims);
        claims = claims.DistinctBy(x => x.Value).ToList();
        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        claims.Add(new Claim(ClaimTypes.Name, user.UserName!));
        claims.Add(new Claim(ClaimTypes.Surname, $"{user.FirstName} {user.LastName}"));

        var expirationTime = DateTime.Now.Date;
        expirationTime = expirationTime.AddMinutes(TimeSpan.FromHours(24).TotalMinutes);

        var tokeOptions = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expirationTime,
            signingCredentials: signinCredentials
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

        return new UserAccessInfoDto()
        {
            Id = user.Id,
            Role = ObjectMapper.Map<Role, RoleDto>(userRoles.FirstOrDefault() ?? new Role()),
            AccessToken = tokenString
        };
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience =
                false, //you might want to validate the audience and issuer depending on your use case
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
            ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }

    private bool IsOnlyRole(List<Guid> ids)
    {
        if (ids.Count < 2) return true;
        return false;
    }

    private void HandleInput(UpdateProfileRequestDto input)
    {
        if (!input.Email.IsNullOrWhiteSpace())
        {
            input.Email = input.Email.Trim();
        }
        else
        {
            input.Email = null;
        }
        if (input.DOB >= DateTime.UtcNow)
        {
            throw new GlobalException(UserErrorCode.InvalidDOB, HttpStatusCode.BadRequest);
        }
        input.PhoneNumber = input.PhoneNumber!.Trim();
    }

    private void HandleInput(CreateUserDto input)
    {
        input.UserName = input.UserName!.Trim();
        input.PhoneNumber = input.PhoneNumber!.Trim();
        input.LastName = input.LastName!.Trim();
        input.FirstName = input.FirstName!.Trim();

        if (!input.Email.IsNullOrWhiteSpace())
        {
            input.Email = input.Email.Trim();
        }
        else
        {
            input.Email = null;
        }
        if (input.DOB >= DateTime.UtcNow)
        {
            throw new GlobalException(UserErrorCode.InvalidDOB, HttpStatusCode.BadRequest);
        }
    }

    private void HandleInput(UpdateUserDto input)
    {
        input.PhoneNumber = input.PhoneNumber!.Trim();
        input.FirstName = input.FirstName!.Trim();
        input.LastName = input.LastName!.Trim();

        if (!input.Email.IsNullOrWhiteSpace())
        {
            input.Email = input.Email.Trim();
        }
        else
        {
            input.Email = null;
        }
        if (input.DOB >= DateTime.UtcNow)
        {
            throw new GlobalException(UserErrorCode.InvalidDOB, HttpStatusCode.BadRequest);
        }
    }

    private async Task<bool> IsUserAllowedAsync(Guid id, Guid currentUserId, string currentRole)
    {
        List<Guid> users = new List<Guid>() { id, currentUserId };
        var listUser = await _unitOfWork.GenericRepository<User>().GetQueryable().Where(x => users.Contains(x.Id)).Select(x => x.Id).ToListAsync();
        var userDetailTask = listUser.FirstOrDefault(id);
        var userCurrentTask = listUser.FirstOrDefault(currentUserId);

        return userDetailTask != userCurrentTask && currentRole == RoleCodes.TECHNICAL_STAFF;
    }

    public async Task<List<BasicUserDto>> GetAllUserAsync(bool isAll = false)
    {
        return await _unitOfWork.GenericRepository<User>()
                    .GetQueryable().AsNoTracking()
                    .WhereIf(!isAll, x => x.IsActive && !x.DeletedAt.HasValue)
                    .OrderBy(x => x.FirstName).ThenBy(x => x.LastName).Select(x => new BasicUserDto
                    {
                        Id = x.Id,
                        FullName = $"{x.FirstName} {x.LastName}",
                        PhoneNumber = x.PhoneNumber!
                    }).ToListAsync();
    }
}