using AutoMapper;
using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.ServiceItems;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Helper;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.ServiceItems;
using CRM_Homestay.Localization;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Volo.Abp.DependencyInjection;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Extensions;
using CRM_Homestay.Entity.BookingServiceItems;
using CRM_Homestay.Entity.HomestayServices;

namespace CRM_Homestay.Service.ServiceItems
{
    public class ServiceItemService : BaseService, IServiceItemService, ITransientDependency
    {
        public ServiceItemService(IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l) : base(unitOfWork, mapper, l)
        {
        }

        public async Task<ServiceItemDto> CreateAsync(CreateServiceItemDto input)
        {
            HandleInput(input);
            var homestayService = await _unitOfWork.GenericRepository<HomestayService>().GetAsync(x => x.Id == input.HomestayServiceId);
            if (homestayService == null)
            {
                throw new GlobalException(code: HomestayServiceErrorCode.NotFound,
                        message: L[HomestayServiceErrorCode.NotFound],
                        statusCode: HttpStatusCode.BadRequest);
            }
            if (input.Type != ServiceItemTypes.Others)
            {
                if (await _unitOfWork.GenericRepository<ServiceItem>().AnyAsync(x => x.Identifier!.ToLower() == input.Identifier!.ToLower()))
                {
                    throw new GlobalException(code: ServiceItemErrorCode.AlreadyExists,
                            message: L[ServiceItemErrorCode.AlreadyExists],
                            statusCode: HttpStatusCode.BadRequest);
                }
            }
            var serviceItem = ObjectMapper.Map<CreateServiceItemDto, ServiceItem>(input);
            serviceItem.NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{input.Identifier} {input.Brand} {input.Model} {input.Type.GetDescription()}");
            await _unitOfWork.GenericRepository<ServiceItem>().AddAsync(serviceItem);

            await _unitOfWork.SaveChangeAsync();
            return ObjectMapper.Map<ServiceItem, ServiceItemDto>(serviceItem);
        }

        public async Task<ServiceItemDto> GetByIdAsync(Guid id)
        {
            var serviceItem = await _unitOfWork.GenericRepository<ServiceItem>()
                                                        .GetQueryable()
                                                        .Include(x => x.HomestayService)
                                                        .AsNoTracking()
                                                        .FirstOrDefaultAsync(x => x.Id == id);
            if (serviceItem == null)
            {
                throw new GlobalException(code: ServiceItemErrorCode.NotFound,
                        message: L[ServiceItemErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            return ObjectMapper.Map<ServiceItem, ServiceItemDto>(serviceItem);
        }

        public async Task<ServiceItemDto> UpdateAsync(Guid id, UpdateServiceItemDto input)
        {
            var serviceItem = await _unitOfWork.GenericRepository<ServiceItem>()
                                                        .GetQueryable()
                                                        .Include(x => x.HomestayService)
                                                        .AsNoTracking()
                                                        .FirstOrDefaultAsync(x => x.Id == id);
            if (serviceItem == null)
            {
                throw new GlobalException(code: ServiceItemErrorCode.NotFound,
                        message: L[ServiceItemErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            if (input.Type != ServiceItemTypes.Others)
            {
                var exists = await _unitOfWork.GenericRepository<ServiceItem>()
                    .AnyAsync(x => x.Id != id && x.Identifier!.ToLower() == input.Identifier!.ToLower());

                if (exists)
                {
                    throw new GlobalException(
                        code: ServiceItemErrorCode.AlreadyExists,
                        message: L[ServiceItemErrorCode.AlreadyExists],
                        statusCode: HttpStatusCode.BadRequest
                    );
                }
            }
            var newServiceItemInfo = ObjectMapper.Map(input, serviceItem);
            serviceItem.NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{input.Identifier} {input.Brand} {input.Model} {input.Type.GetDescription()}");

            _unitOfWork.GenericRepository<ServiceItem>().Update(newServiceItemInfo);
            await _unitOfWork.SaveChangeAsync();

            return ObjectMapper.Map<ServiceItem, ServiceItemDto>(serviceItem);
        }

        public async Task<PagedResultDto<ServiceItemDto>> GetPagingWithFilterAsync(ServiceItemFilterDto filter)
        {
            string searchTerm = !string.IsNullOrEmpty(filter.Text) ? $" {NormalizeString.ConvertNormalizeString(filter.Text.Trim())} " : string.Empty;

            var query = _unitOfWork.GenericRepository<ServiceItem>()
                .GetQueryable()
                .AsNoTracking()
                .Include(x => x.HomestayService)
                .WhereIf(filter.Type.HasValue, r => r.Type == filter.Type!.Value)
                .WhereIf(!string.IsNullOrEmpty(searchTerm),
                    x => x.NormalizeFullInfo!.Contains(searchTerm))
                .OrderByDescending(x => x.CreationTime);

            var data = await query.GetPaged(filter.CurrentPage, filter.PageSize);
            var result = ObjectMapper.Map<PagedResult<ServiceItem>, PagedResultDto<ServiceItemDto>>(data);

            return result;
        }

        public async Task DeleteAsync(Guid id)
        {
            var serviceItem = await _unitOfWork.GenericRepository<ServiceItem>().GetAsync(x => x.Id == id);
            if (serviceItem == null)
            {
                throw new GlobalException(code: ServiceItemErrorCode.NotFound,
                        message: L[ServiceItemErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            if (await HasAnyActiveBooking(id))
            {
                throw new GlobalException(code: ServiceItemErrorCode.NotAllowedDelete,
                        message: L[ServiceItemErrorCode.NotAllowedDelete],
                        statusCode: HttpStatusCode.BadRequest);
            }
            _unitOfWork.GenericRepository<ServiceItem>().Remove(serviceItem);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<List<ServiceItemDto>> GetAllAsync()
        {
            var serviceItems = await _unitOfWork.GenericRepository<ServiceItem>()
                .GetQueryable()
                .AsNoTracking()
                .Include(x => x.HomestayService)
                .OrderByDescending(x => x.CreationTime)
                .ToListAsync();
            return ObjectMapper.Map<List<ServiceItemDto>>(serviceItems);
        }

        public async Task<List<ServiceItemDto>> GetByHomestayServiceIdAsync(Guid serviceId)
        {
            var serviceItems = await _unitOfWork.GenericRepository<ServiceItem>()
                .GetQueryable()
                .AsNoTracking()
                .Include(x => x.HomestayService)
                .Where(x => x.HomestayServiceId == serviceId)
                .OrderByDescending(x => x.CreationTime)
                .ToListAsync();

            return ObjectMapper.Map<List<ServiceItemDto>>(serviceItems);
        }

        private async Task<bool> HasAnyActiveBooking(Guid id)
        {
            var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            return await _unitOfWork.GenericRepository<BookingServiceItem>()
                .AnyAsync(x =>
                    x.ServiceItemId == id &&
                    x.StartDate <= now &&
                    (!x.EndDate.HasValue || x.EndDate >= now)
                );
        }
        private void HandleInput(CreateServiceItemDto input)
        {
            input.Identifier = input.Identifier != null ? input.Identifier.Trim() : null;
            input.Brand = input.Brand != null ? input.Brand.Trim() : null;
            input.Model = input.Model != null ? input.Model.Trim() : null;
            input.Description = input.Description != null ? input.Description.Trim() : null;
        }
    }
}
