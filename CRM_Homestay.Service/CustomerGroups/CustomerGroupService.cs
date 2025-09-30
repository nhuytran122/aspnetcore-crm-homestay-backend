using AutoMapper;
using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.CustomerGroups;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Helper;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.CustomerGroups;
using CRM_Homestay.Entity.Customers;
using CRM_Homestay.Localization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CRM_Homestay.Service.CustomerGroups;

public class CustomerGroupService : BaseService, ICustomerGroupService
{
    public CustomerGroupService([NotNull] IUnitOfWork unitOfWork, [NotNull] IMapper mapper, [NotNull] ILocalizer l) :
        base(unitOfWork, mapper, l)
    {
    }

    public async Task<PagedResultDto<CustomerGroupDto>> GetListAsync(CustomerGroupFilterDto filter)
    {
        string searchTerm = !string.IsNullOrEmpty(filter.Text) ? $" {NormalizeString.ConvertNormalizeString(filter.Text.Trim())} " : string.Empty;
        var pagedGroups = await _unitOfWork.GenericRepository<CustomerGroup>()
            .GetQueryable()
            .WhereIf(filter.IsActive != null, x => x.IsActive == filter.IsActive)
            .WhereIf(!string.IsNullOrEmpty(filter.Text),
                x => x.NormalizeFullInfo!.Contains(searchTerm) ||
                x.NormalizeFullInfo.Contains(filter.Text!.Trim().ToUpper()) ||
                x.Name!.ToUpper().Contains(filter.Text.Trim().ToUpper()))
            .OrderByDescending(x => x.CreationTime)
            .GetPaged(filter.CurrentPage, filter.PageSize);

        return ObjectMapper.Map<PagedResult<CustomerGroup>, PagedResultDto<CustomerGroupDto>>(pagedGroups);
    }

    public async Task<List<CustomerGroupDto>> GetListDropdownAsync()
    {
        return await _unitOfWork.GenericRepository<CustomerGroup>()
            .GetQueryable()
            .Where(x => x.IsActive == true)
            .OrderByDescending(x => x.CreationTime)
            .Select(x => new CustomerGroupDto()
            {
                Id = x.Id,
                Name = x.Name,
            })
            .ToListAsync();
    }

    public async Task<CustomerGroupDto> CreateAsync(CreateUpdateCustomerGroupDto input)
    {
        HandleInput(input);

        var repo = _unitOfWork.GenericRepository<CustomerGroup>();

        var customerGroups = await repo
            .GetListAsync(x => x.NormalizedCode == input.Code!.ToUpper()
                               || x.NormalizedName == input.Name!.ToUpper());


        if (customerGroups.Any(x => x.NormalizedName == input.Name!.ToUpper()))
        {
            throw new GlobalException(L[CustomerGroupErrorCode.NameAlreadyExist], HttpStatusCode.BadRequest);
        }

        if (customerGroups.Any(x => x.NormalizedCode == input.Code!.ToUpper()))
        {
            throw new GlobalException(L[CustomerGroupErrorCode.CodeAlreadyExist], HttpStatusCode.BadRequest);
        }
        if (customerGroups.Any(x => x.MinPoints == input.MinPoints))
        {
            throw new GlobalException(L[CustomerGroupErrorCode.MinPointsAlreadyExist], HttpStatusCode.BadRequest);
        }

        var group = ObjectMapper.Map<CreateUpdateCustomerGroupDto, CustomerGroup>(input);
        group.NormalizeFullInfo = NormalizeString.ConvertNormalizeString(input.Name!);

        await repo.AddAsync(group);
        await _unitOfWork.SaveChangeAsync();
        return ObjectMapper.Map<CustomerGroup, CustomerGroupDto>(group);
    }

    public async Task<CustomerGroupDto> UpdateAsync(CreateUpdateCustomerGroupDto input, Guid id)
    {
        HandleInput(input);
        var repo = _unitOfWork.GenericRepository<CustomerGroup>();

        var group = await repo.GetAsync(x => x.Id == id);

        if (group == null)
        {
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.BadRequest);
        }

        var customerGroups = await repo
            .GetListAsync(x => x.NormalizedCode == input.Code!.ToUpper()
                               || x.NormalizedName == input.Name!.ToUpper());

        if (customerGroups.Any(x => x.NormalizedName == input.Name!.ToUpper() && x.Id != id))
        {
            throw new GlobalException(L[CustomerGroupErrorCode.NameAlreadyExist], HttpStatusCode.BadRequest);
        }

        if (customerGroups.Any(x => x.NormalizedCode == input.Code!.ToUpper() && x.Id != id))
        {
            throw new GlobalException(L[CustomerGroupErrorCode.CodeAlreadyExist], HttpStatusCode.BadRequest);
        }
        if (customerGroups.Any(x => x.MinPoints == input.MinPoints && x.Id != id))
        {
            throw new GlobalException(L[CustomerGroupErrorCode.MinPointsAlreadyExist], HttpStatusCode.BadRequest);
        }
        group = ObjectMapper.Map(input, group);
        group.NormalizeFullInfo = NormalizeString.ConvertNormalizeString(input.Name!);

        repo.Update(group);
        await _unitOfWork.SaveChangeAsync();
        return ObjectMapper.Map<CustomerGroup, CustomerGroupDto>(group);
    }

    public async Task DeleteAsync(Guid id)
    {
        var groupRepo = _unitOfWork.GenericRepository<CustomerGroup>();
        var group = await groupRepo.GetAsync(x => x.Id == id);
        if (group == null)
        {
            throw new GlobalException(L[BaseErrorCode.NotFound], HttpStatusCode.BadRequest);
        }
        if (await _unitOfWork.GenericRepository<Customer>()
                .GetQueryable().IgnoreQueryFilters().AnyAsync(x => x.GroupId == id) || group.MinPoints == 0)
        {
            throw new GlobalException(L[CustomerGroupErrorCode.NotDelete], HttpStatusCode.BadRequest);
        }

        groupRepo.Remove(group);
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task<string> GenerateCode()
    {
        var count = await _unitOfWork.GenericRepository<CustomerGroup>().GetCountAsync();
        return $"GR{count + 1:D5}";
    }

    private void HandleInput(CreateUpdateCustomerGroupDto input)
    {
        string[] words = input.Name!.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        input.Name = string.Join(" ", words);
        input.Code = input.Code!.Trim();
    }
}