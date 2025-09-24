using AutoMapper;
using CRM_Homestay.Contract.Claims;
using CRM_Homestay.Contract.Roles;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Consts.Permissions;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.RoleClaims;
using CRM_Homestay.Entity.Roles;
using CRM_Homestay.Localization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Security.Claims;

namespace CRM_Homestay.Service.Roles;

public class RoleService : BaseService, IRoleService
{
    private RoleManager<Role> _roleManager;


    public RoleService(IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l, RoleManager<Role> roleManager) : base(unitOfWork, mapper, l)
    {
        _roleManager = roleManager;

    }

    public async Task<List<RoleDto>> GetListAsync()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        var rolesDto = ObjectMapper.Map<List<Role>, List<RoleDto>>(roles);
        return rolesDto;
    }

    public async Task<RoleDto> CreateAsync(CreateUpdateRoleDto input, bool save = true)
    {
        HandleInput(input);
        var role = ObjectMapper.Map<CreateUpdateRoleDto, Role>(input);

        if (await _unitOfWork.GenericRepository<Role>().AnyAsync(x => x.NormalizedName == input.Name!.ToUpper()))
        {
            throw new GlobalException(L[RoleErrorCode.NameAlreadyExist], HttpStatusCode.BadRequest);
        }

        role.NormalizedName = input.Name!.ToUpper();
        await _unitOfWork.GenericRepository<Role>().AddAsync(role);
        if (save)
        {
            await _unitOfWork.SaveChangeAsync();
        }

        return ObjectMapper.Map<Role, RoleDto>(role);
    }

    public async Task<RoleDto> UpdateAsync(CreateUpdateRoleDto input, Guid id, bool save = true)
    {
        HandleInput(input);
        var item = await _roleManager.FindByIdAsync(id.ToString());

        if (item == null)
        {
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.NotFound);

        }
        if (await _unitOfWork.GenericRepository<Role>().AnyAsync(x => x.NormalizedName == input.Name!.ToUpper() && x.Id != id))
        {
            throw new GlobalException(L[RoleErrorCode.NameAlreadyExist], HttpStatusCode.BadRequest);
        }

        var role = ObjectMapper.Map(input, item);
        role.NormalizedName = input.Name!.ToUpper();
        _unitOfWork.GenericRepository<Role>().Update(role);
        if (save)
        {
            await _unitOfWork.SaveChangeAsync();
        }
        return ObjectMapper.Map<Role, RoleDto>(role);
    }

    public async Task DeleteAsync(Guid id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null)
        {
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.BadRequest);
        }

        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            throw new GlobalException(L[result.Errors?.FirstOrDefault()!.Code!], HttpStatusCode.BadRequest);
        }
    }

    public async Task UpdateClaimsToRole(Guid id, List<CreateUpdateClaimDto> claims)
    {
        var role = await _unitOfWork.GenericRepository<Role>().GetAsync(x => x.Id == id);
        if (role == null)
        {
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.BadRequest);

        }

        if (CheckAllowedClaimType(claims))
        {
            throw new GlobalException(L[BaseErrorCode.InvalidRequirement], HttpStatusCode.BadRequest);
        }

        var oldClaims = await _unitOfWork.GenericRepository<RoleClaim>()
            .GetListAsync(x => x.RoleId == id);
        _unitOfWork.GenericRepository<RoleClaim>().RemoveRange(oldClaims);

        var newRoleClaims = new List<RoleClaim>();
        foreach (var item in claims)
        {
            newRoleClaims.Add(new RoleClaim()
            {
                RoleId = id,
                ClaimType = item.ClaimType,
                ClaimValue = item.ClaimValue
            });
        }
        _unitOfWork.GenericRepository<RoleClaim>().AddRange(newRoleClaims);
        await _unitOfWork.SaveChangeAsync();

    }

    public async Task<List<ClaimDto>> GetClaims(Guid roleId)
    {
        var role = await _unitOfWork.GenericRepository<Role>()
            .GetAsync(x => x.Id == roleId);
        if (role == null)
        {
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.BadRequest);
        }
        var result = (List<Claim>)await _roleManager.GetClaimsAsync(role);



        return ObjectMapper.Map<List<Claim>, List<ClaimDto>>(result);
    }

    public async Task CreateWithClaimsAsync(CreateUpdateRoleDto input)
    {
        var role = await CreateAsync(input, false);


        if (CheckAllowedClaimType(input.Claims))
        {
            throw new GlobalException(L[BaseErrorCode.InvalidRequirement], HttpStatusCode.BadRequest);
        }

        var roleClaims = new List<RoleClaim>();
        foreach (var item in input.Claims)
        {
            roleClaims.Add(new RoleClaim()
            {
                RoleId = role.Id,
                ClaimType = item.ClaimType,
                ClaimValue = item.ClaimValue
            });
        }

        _unitOfWork.GenericRepository<RoleClaim>().AddRange(roleClaims);
        await _unitOfWork.SaveChangeAsync();


    }

    public async Task UpdateWithClaimsAsync(CreateUpdateRoleDto input, Guid id)
    {
        await UpdateAsync(input, id, false);

        if (CheckAllowedClaimType(input.Claims))
        {
            throw new GlobalException(L[BaseErrorCode.InvalidRequirement], HttpStatusCode.BadRequest);
        }

        var oldClaims = await _unitOfWork.GenericRepository<RoleClaim>()
            .GetListAsync(x => x.RoleId == id);
        _unitOfWork.GenericRepository<RoleClaim>().RemoveRange(oldClaims);

        var newRoleClaims = new List<RoleClaim>();
        foreach (var item in input.Claims)
        {
            newRoleClaims.Add(new RoleClaim()
            {
                RoleId = id,
                ClaimType = item.ClaimType,
                ClaimValue = item.ClaimValue
            });
        }
        _unitOfWork.GenericRepository<RoleClaim>().AddRange(newRoleClaims);
        await _unitOfWork.SaveChangeAsync();

    }


    private void HandleInput(CreateUpdateRoleDto input)
    {
        input.Name = input.Name!.Trim();
    }

    private bool CheckAllowedClaimType(List<CreateUpdateClaimDto> Claims)
    {
        var types = new List<string>()
            {
                ExtendClaimTypes.Permission
            };

        var invalidTypes = Claims.Select(x => x.ClaimType)
             .Distinct()
            .Except(types);
        if (invalidTypes.Any())
        {
            return true;
        }

        return false;
    }
}