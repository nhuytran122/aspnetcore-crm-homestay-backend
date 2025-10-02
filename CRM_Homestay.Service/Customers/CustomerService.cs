using AutoMapper;
using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.CustomerGroups;
using CRM_Homestay.Contract.Customers;
using CRM_Homestay.Contract.Locations;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Helper;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.Bookings;
using CRM_Homestay.Entity.CustomerGroups;
using CRM_Homestay.Entity.Customers;
using CRM_Homestay.Localization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CRM_Homestay.Service.Customers;

public class CustomerService : BaseService, ICustomerService
{
    private ILocationServiceShared _locationServiceShared;

    public CustomerService(ILocationServiceShared locationServiceShared, [NotNull] IUnitOfWork unitOfWork,
        [NotNull] IMapper mapper, [NotNull] ILocalizer l) : base(unitOfWork, mapper, l)
    {
        _locationServiceShared = locationServiceShared;
    }

    public async Task<PagedResultDto<CustomerWithNavigationPropertiesDto>> GetListWithNavigationPropertiesAsync(
        CustomerFilterDto input)
    {
        if (!input.IsValidFilter())
        {
            throw new GlobalException(L[BaseErrorCode.InvalidRequirement], HttpStatusCode.BadRequest);
        }

        var searchTerm = !string.IsNullOrEmpty(input.Text)
            ? $" {NormalizeString.ConvertNormalizeString(input.Text)} "
            : string.Empty;
        var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        var query = _unitOfWork.GenericRepository<Customer>()
            .GetQueryable()
            .Include(x => x.Group)
            .Include(x => x.Bookings)
            .OrderByDescending(x => x.CreationTime)
            .Where(x => !x.DeletedAt.HasValue)
            .WhereIf(!string.IsNullOrEmpty(input.Text),
                x => (" " + x.NormalizeFullInfo + " ").Contains(searchTerm) ||
                     (" " + x.PhoneNumber + " ").Contains(searchTerm))
            .WhereIf(input.GroupId != null, x => x.GroupId == input.GroupId)
            .WhereIf(input.Type != null, x => x.Type == input.Type)
            .WhereIf(input.LastVisitFrom.HasValue || input.LastVisitTo.HasValue,
                x => x.Bookings!.Any(b =>
                    b.CheckOut <= now &&
                    (!input.LastVisitFrom.HasValue || b.CheckOut >= input.LastVisitFrom.Value) &&
                    (!input.LastVisitTo.HasValue || b.CheckOut <= input.LastVisitTo.Value)
            ))
            .WhereIf(input.NextVisitFrom.HasValue || input.NextVisitTo.HasValue,
                x => x.Bookings!.Any(b =>
                    b.CheckIn >= now &&
                    (!input.NextVisitFrom.HasValue || b.CheckIn >= input.NextVisitFrom.Value) &&
                    (!input.NextVisitTo.HasValue || b.CheckIn <= input.NextVisitTo.Value
            )))
            .WhereIf(input.IsGatePassSent.HasValue, x =>
                x.Bookings!
                    .Where(b => b.CheckIn >= now)
                    .OrderBy(b => b.CheckIn)
                    .Select(b => b.IsGatePassSent)
                    .FirstOrDefault() == input.IsGatePassSent!.Value
            )
            .Select(c => new CustomerWithNavigationPropertiesDto
            {
                Customer = ObjectMapper.Map<CustomerDto>(c),
                Group = ObjectMapper.Map<CustomerGroupDto>(c.Group),
                IsDeletable = !c.Bookings!.Any(),
                NextDateVisit = c.Bookings!
                    .Where(b => b.CheckIn >= now)
                    .OrderBy(b => b.CheckIn)
                    .Select(b => b.CheckIn)
                    .FirstOrDefault(),
                IsGatePassSent = c.Bookings!
                    .Where(b => b.CheckIn >= now)
                    .OrderBy(b => b.CheckIn)
                    .Select(b => b.IsGatePassSent)
                    .FirstOrDefault()
            });
        var data = await query.GetPaged(input.CurrentPage, input.PageSize);
        return ObjectMapper.Map<PagedResult<CustomerWithNavigationPropertiesDto>, PagedResultDto<CustomerWithNavigationPropertiesDto>>(data);
    }

    public async Task<PagedResultDto<CustomerDto>> GetCustomerWithFilterAsync(string? text)
    {
        var query = _unitOfWork.GenericRepository<Customer>()
            .GetQueryable()
            .OrderByDescending(x => x.CreationTime)
            .Include(x => x.Group)
            .Select(x => new CustomerDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                FullName = x.Type == CustomerTypes.Individual ?
                            x.FirstName + " " + x.LastName : x.CompanyName,
                DOB = x.DOB,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Gender = x.Gender,
                Type = x.Type,
                CompanyName = x.CompanyName,
                TaxCode = x.TaxCode,
                GroupId = x.GroupId,
                GroupName = x.Group!.Name,
                Address = x.Address!,
                NormalizeFullInfo = x.NormalizeFullInfo,
                NormalizeAddress = x.NormalizeAddress
            });

        if (!string.IsNullOrEmpty(text))
        {
            var filter = text.Split(',').Select(item => item.Trim().ToLower()).ToList();

            var searchTerm1 = filter.Count >= 1 ? NormalizeString.ConvertNormalizeString(filter[0]) : string.Empty;
            var searchTerm2 = filter.Count >= 2 ? NormalizeString.ConvertNormalizeString(filter[1]) : string.Empty;
            var searchTerm3 = filter.Count >= 3 ? NormalizeString.ConvertNormalizeString(filter[2]) : string.Empty;

            query = query
                .WhereIf(!string.IsNullOrEmpty(searchTerm1),
                    x => (" " + x.NormalizeFullInfo + " ").Contains(searchTerm1) ||
                         (" " + x.PhoneNumber + " ").Contains(searchTerm1) ||
                         (" " + x.NormalizeAddress + " ").Contains(searchTerm1))
                .WhereIf(!string.IsNullOrEmpty(searchTerm2),
                    x => (" " + x.NormalizeFullInfo + " ").Contains(searchTerm2) &&
                         (" " + x.PhoneNumber + " ").Contains(searchTerm2))
                .WhereIf(!string.IsNullOrEmpty(searchTerm3),
                    x => (" " + x.NormalizeAddress + " ").Contains(searchTerm3));
        }
        var data = await query.GetPaged(1, 50);
        return ObjectMapper.Map<PagedResult<CustomerDto>, PagedResultDto<CustomerDto>>(data);
    }

    public async Task<List<CustomerDto>> GetListAsync()
    {
        var query = _unitOfWork.GenericRepository<Customer>()
            .GetQueryable()
            .OrderByDescending(x => x.CreationTime);

        var customers = await query
            .Select(c => ObjectMapper.Map<Customer, CustomerDto>(c))
            .ToListAsync();

        return customers;
    }

    public async Task<CustomerDto> CreateAsync(CreateUpdateCustomerDto input)
    {
        HandleInput(input);
        var customerRepo = _unitOfWork.GenericRepository<Customer>();

        var customers = await customerRepo
            .GetQueryable()
            .WhereIf(input.Type == CustomerTypes.Individual, x =>
                (!input.Email.IsNullOrWhiteSpace() && x.NormalizedEmail == input.Email.ToUpper())
                )
            .WhereIf(input.Type == CustomerTypes.Organization
                , x => (x.NormalizedCompanyName == input.CompanyName!.ToUpper())
                       || x.TaxCode == input.TaxCode
                       || (!input.Email.IsNullOrWhiteSpace() && x.NormalizedEmail == input.Email.ToUpper())
                       ).ToListAsync();

        var customer = ObjectMapper.Map<CreateUpdateCustomerDto, Customer>(input);

        if (!input.Email.IsNullOrWhiteSpace())
        {
            if (customers.Any(x => x.NormalizedEmail == input.Email.ToUpper()))
            {
                throw new GlobalException(L[BaseErrorCode.EmailAlreadyExist], HttpStatusCode.BadRequest);
            }
        }

        if (input.Type == CustomerTypes.Organization)
        {
            if (customers.Any(x => x.NormalizedCompanyName == input.CompanyName!.ToUpper()))
            {
                throw new GlobalException(L[BaseErrorCode.CompanyNameAlreadyExist], HttpStatusCode.BadRequest);
            }

            if (customers.Any(x => x.TaxCode == input.TaxCode))
            {
                throw new GlobalException(L[BaseErrorCode.TaxCodeAlreadyExist], HttpStatusCode.BadRequest);
            }
        }

        if (input.GroupId == null || input.GroupId == Guid.Empty)
        {
            var group = await _unitOfWork.GenericRepository<CustomerGroup>()
                .GetQueryable()
                .OrderBy(g => g.MinPoints)
                .FirstOrDefaultAsync();
            if (group == null)
            {
                throw new GlobalException(code: CustomerGroupErrorCode.NotFound,
                        message: L[CustomerGroupErrorCode.NotFound],
                        statusCode: HttpStatusCode.BadRequest);
            }
            customer.GroupId = group.Id;
        }
        else
        {
            var group = await _unitOfWork.GenericRepository<CustomerGroup>().GetAsync(x => x.Id == input.GroupId);
            if (group == null)
            {
                throw new GlobalException(code: CustomerGroupErrorCode.NotFound,
                        message: L[CustomerGroupErrorCode.NotFound],
                        statusCode: HttpStatusCode.BadRequest);
            }
            customer.GroupId = input.GroupId!.Value;
        }
        customer.PhoneNumber = string.Join("/", input.PhoneNumber!);

        var location = await _locationServiceShared.GetLocations(new LocationRequestDto()
        {
            ProvinceId = input.Address.ProvinceId,
            DistrictId = input.Address.DistrictId,
            WardId = input.Address.WardId,
            Locate = input.Address.Locate
        });

        var addressSpecifically = string.Empty;
        if (location?.Ward != null) addressSpecifically = $"{location.Ward}";
        if (location?.District != null) addressSpecifically += $", {location.District}";
        if (location?.Province != null) addressSpecifically += $", {location.Province}";

        if (!string.IsNullOrEmpty(input.Address.Street))
        {
            customer.Address!.JoinedName = input.Address.Street;
            if (!string.IsNullOrEmpty(addressSpecifically))
            {
                customer.Address.JoinedName += ", " + addressSpecifically;
            }
        }
        else
        {
            customer.Address!.JoinedName = addressSpecifically;
        }
        customer.Address.WardName = location?.Ward;
        customer.Address.DistrictName = location?.District;
        customer.Address.ProvinceName = location?.Province;
        customer.Address.Locate = location?.Locate;
        customer.NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{input.FirstName} {input.LastName} {input.CompanyName}");
        customer.NormalizeAddress = NormalizeString.ConvertNormalizeString(customer.Address.JoinedName.Replace(",", ""));

        await _unitOfWork.GenericRepository<Customer>().AddAsync(customer);
        await _unitOfWork.SaveChangeAsync();
        return ObjectMapper.Map<Customer, CustomerDto>(customer);
    }

    public async Task<CustomerDto> UpdateAsync(CreateUpdateCustomerDto input, Guid id)
    {
        HandleInput(input);
        var customerRepo = _unitOfWork.GenericRepository<Customer>();

        var customer = await customerRepo.GetAsync(x => x.Id == id);
        if (customer == null)
        {
            throw new GlobalException(code: CustomerErrorCode.NotFound,
                        message: L[CustomerErrorCode.NotFound],
                        statusCode: HttpStatusCode.BadRequest);
        }

        var customers = await customerRepo
            .GetQueryable()
            .WhereIf(input.Type == CustomerTypes.Individual, x =>
                (!input.Email.IsNullOrWhiteSpace() && x.NormalizedEmail == input.Email.ToUpper())
                )
            .WhereIf(input.Type == CustomerTypes.Organization
                , x => (x.NormalizedCompanyName == input.CompanyName!.ToUpper())
                      || x.TaxCode == input.TaxCode
                      || (!input.Email.IsNullOrWhiteSpace() && x.NormalizedEmail == input.Email.ToUpper())
                      ).ToListAsync();

        if (!input.Email.IsNullOrWhiteSpace())
        {
            if (await customerRepo.AnyAsync(x => x.NormalizedEmail == input.Email.ToUpper() && x.Id != id))
            {
                throw new GlobalException(L[BaseErrorCode.EmailAlreadyExist], HttpStatusCode.BadRequest);
            }
        }

        if (input.Type == CustomerTypes.Organization)
        {
            if (customers.Any(x => x.NormalizedCompanyName == input.CompanyName && x.Id != id))
            {
                throw new GlobalException(L[BaseErrorCode.CompanyNameAlreadyExist], HttpStatusCode.BadRequest);
            }

            if (customers.Any(x => x.TaxCode == input.TaxCode && x.Id != id))
            {
                throw new GlobalException(L[BaseErrorCode.TaxCodeAlreadyExist], HttpStatusCode.BadRequest);
            }
        }
        customer = ObjectMapper.Map(input, customer);
        if (input.GroupId == null || input.GroupId == Guid.Empty)
        {
            var group = await _unitOfWork.GenericRepository<CustomerGroup>()
                .GetQueryable()
                .OrderBy(g => g.MinPoints)
                .FirstOrDefaultAsync();
            if (group == null)
            {
                throw new GlobalException(code: CustomerGroupErrorCode.NotFound,
                        message: L[CustomerGroupErrorCode.NotFound],
                        statusCode: HttpStatusCode.BadRequest);
            }
            customer.GroupId = group.Id;
        }
        else
        {
            var group = await _unitOfWork.GenericRepository<CustomerGroup>().GetAsync(x => x.Id == input.GroupId);
            if (group == null)
            {
                throw new GlobalException(code: CustomerGroupErrorCode.NotFound,
                        message: L[CustomerGroupErrorCode.NotFound],
                        statusCode: HttpStatusCode.BadRequest);
            }
            customer.GroupId = input.GroupId!.Value;
        }
        customer.PhoneNumber = string.Join("/", input.PhoneNumber!);

        var location = await _locationServiceShared.GetLocations(new LocationRequestDto()
        {
            ProvinceId = input.Address.ProvinceId,
            DistrictId = input.Address.DistrictId,
            WardId = input.Address.WardId,
            Locate = input.Address.Locate
        });

        var addressSpecifically = string.Empty;
        if (location?.Ward != null) addressSpecifically = $"{location.Ward}";
        if (location?.District != null) addressSpecifically += $", {location.District}";
        if (location?.Province != null) addressSpecifically += $", {location.Province}";
        if (!string.IsNullOrEmpty(input.Address.Street))
        {
            customer.Address!.JoinedName = input.Address.Street;
            if (!string.IsNullOrEmpty(addressSpecifically))
            {
                customer.Address.JoinedName += ", " + addressSpecifically;
            }
        }
        else
        {
            customer.Address!.JoinedName = addressSpecifically;
        }
        customer.Address.WardName = location?.Ward;
        customer.Address.DistrictName = location?.District;
        customer.Address.ProvinceName = location?.Province;
        customer.Address.Locate = location?.Locate;
        customer.NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{input.FirstName} {input.LastName} {input.CompanyName}");
        customer.NormalizeAddress = NormalizeString.ConvertNormalizeString(customer.Address.JoinedName);

        customerRepo.Update(customer);
        await _unitOfWork.SaveChangeAsync();
        return ObjectMapper.Map<Customer, CustomerDto>(customer);
    }

    public async Task DeleteAsync(Guid id)
    {
        var repo = _unitOfWork.GenericRepository<Customer>();
        var customer = await repo.GetAsync(x => x.Id == id);
        if (customer == null)
        {
            throw new GlobalException(code: CustomerErrorCode.NotFound,
                        message: L[CustomerErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
        }

        if (await _unitOfWork
            .GenericRepository<Booking>()
            .AnyAsync(x => x.CustomerId == id))
        {
            throw new GlobalException(code: CustomerErrorCode.NotDelete,
                        message: L[CustomerErrorCode.NotDelete],
                        statusCode: HttpStatusCode.BadRequest);
        }
        else
        {
            customer.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            _unitOfWork.GenericRepository<Customer>().Update(customer);
            await _unitOfWork.SaveChangeAsync();
        }
    }

    private void HandleInput(CreateUpdateCustomerDto input)
    {
        if (!input.CompanyName.IsNullOrWhiteSpace())
        {
            string[] words = input.CompanyName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            input.CompanyName = string.Join(" ", words);
        }

        if (!input.Email.IsNullOrWhiteSpace())
        {
            input.Email = input.Email.Trim();
        }
        if (input.Type == CustomerTypes.Individual)
        {
            input.CompanyName = null;
            input.TaxCode = null;
        }
        else
        {
            input.FirstName = null;
            input.LastName = null;
        }
    }

    public async Task<CustomerDto> GetByIdAsync(Guid id)
    {
        var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        var customer = await _unitOfWork.GenericRepository<Customer>()
            .GetQueryable()
            .AsNoTracking()
            .Include(x => x.Group)
            .Include(x => x.CustomerAccount)
            .Include(x => x.Bookings)
            .Where(x => x.Id == id)
            .Select(x => new CustomerDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                
                
                DOB = x.DOB,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Gender = x.Gender,
                Type = x.Type,
                CompanyName = x.CompanyName,
                TaxCode = x.TaxCode,
                Address = x.Address!,
                GroupId = x.GroupId,
                GroupName = x.Group != null ? x.Group.Name : null,
                NormalizeFullInfo = x.NormalizeFullInfo,
                NormalizeAddress = x.NormalizeAddress,
                CustomerAccount = x.CustomerAccount,

                LastVisit = x.Bookings!
                    .Where(b => b.CheckIn <= now && !b.DeletedAt.HasValue)
                    .OrderByDescending(b => b.CheckIn)
                    .Select(b => b.CheckIn)
                    .FirstOrDefault(),

                NextVisit = x.Bookings!
                    .Where(b => b.CheckIn > now && !b.DeletedAt.HasValue)
                    .OrderBy(b => b.CheckIn)
                    .Select(b => b.CheckIn)
                    .FirstOrDefault(),

                IsGatePassSent = x.Bookings!
                    .Where(b => b.CheckIn > now && !b.DeletedAt.HasValue)
                    .OrderBy(b => b.CheckIn)
                    .Select(b => b.IsGatePassSent)
                    .FirstOrDefault(),

                TotalPaid = x.Bookings!
                    .Where(b => !b.DeletedAt.HasValue)
                    .Sum(b => b.PaidAmount),

                TotalVisited = x.Bookings!
                    .Where(b => b.CheckIn <= now && !b.DeletedAt.HasValue)
                    .Count()
            })
            .FirstOrDefaultAsync();

        return customer!;
    }

}