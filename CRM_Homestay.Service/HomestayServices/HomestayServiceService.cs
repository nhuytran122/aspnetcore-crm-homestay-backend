using AutoMapper;
using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.HomestayServices;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Helper;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.HomestayServices;
using CRM_Homestay.Localization;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Volo.Abp.DependencyInjection;
using CRM_Homestay.Contract.Uploads;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Entity.ServiceItems;
using CRM_Homestay.Core.Extensions;
using CRM_Homestay.Entity.BookingServices;
using CRM_Homestay.Contract.ServiceItems;

namespace CRM_Homestay.Service.HomestayServices
{
    public class HomestayServiceService : BaseService, IHomestayServiceService, ITransientDependency
    {
        private readonly IUploadService _uploadService;
        public HomestayServiceService(IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l, IUploadService uploadService) : base(unitOfWork, mapper, l)
        {
            _uploadService = uploadService;
        }

        public async Task<HomestayServiceDto> CreateAsync(CreateUpdateHomestayServiceDto input)
        {
            if (await _unitOfWork.GenericRepository<HomestayService>().AnyAsync(x => x.Name == input.Name))
            {
                throw new GlobalException(code: HomestayServiceErrorCode.AlreadyExists,
                        message: L[HomestayServiceErrorCode.AlreadyExists],
                        statusCode: HttpStatusCode.BadRequest);
            }
            HandleInput(input);

            var homestayService = ObjectMapper.Map<CreateUpdateHomestayServiceDto, HomestayService>(input);

            if (input.ServiceItems != null && input.ServiceItems.Any())
            {
                homestayService.ServiceItems = input.ServiceItems.Select(item => new ServiceItem
                {
                    HomestayServiceId = homestayService.Id,
                    Identifier = item.Identifier,
                    Brand = item.Brand,
                    Model = item.Model,
                    Type = item.Type,
                    Description = item.Description,
                    NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{item.Identifier} {item.Brand} {item.Model} {item.Type.GetDescription()}")
                }).ToList();
            }
            homestayService.NormalizeFullInfo = NormalizeString.ConvertNormalizeString(input.Name!);
            await _unitOfWork.GenericRepository<HomestayService>().AddAsync(homestayService);
            await _unitOfWork.SaveChangeAsync();
            return ObjectMapper.Map<HomestayService, HomestayServiceDto>(homestayService);
        }

        public async Task<HomestayServiceDetailDto> GetByIdAsync(Guid id)
        {
            var homestayService = await _unitOfWork.GenericRepository<HomestayService>()
                .GetQueryable()
                .AsNoTracking()
                .Include(x => x.ServiceItems!)
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeletedAt.HasValue);

            if (homestayService == null)
            {
                throw new GlobalException(
                    code: HomestayServiceErrorCode.NotFound,
                    message: L[HomestayServiceErrorCode.NotFound],
                    statusCode: HttpStatusCode.NotFound
                );
            }
            return ObjectMapper.Map<HomestayService, HomestayServiceDetailDto>(homestayService);
        }

        public async Task<HomestayServiceDto> UpdateAsync(Guid id, CreateUpdateHomestayServiceDto input)
        {
            var homestayService = await _unitOfWork.GenericRepository<HomestayService>()
                .GetQueryable()
                .Include(x => x.ServiceItems)
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeletedAt.HasValue);

            if (homestayService == null)
                throw new GlobalException(
                    code: HomestayServiceErrorCode.NotFound,
                    message: L[HomestayServiceErrorCode.NotFound],
                    statusCode: HttpStatusCode.NotFound);

            if (await _unitOfWork.GenericRepository<HomestayService>()
                .AnyAsync(x => x.Id != id && x.Name == input.Name))
                throw new GlobalException(
                    code: HomestayServiceErrorCode.AlreadyExists,
                    message: L[HomestayServiceErrorCode.AlreadyExists],
                    statusCode: HttpStatusCode.BadRequest);
            HandleInput(input);
            using (_unitOfWork.BeginTransaction())
            {
                try
                {
                    ObjectMapper.Map(input, homestayService);
                    homestayService.NormalizeFullInfo = NormalizeString.ConvertNormalizeString(input.Name!);

                    var currentItems = homestayService.ServiceItems ?? new List<ServiceItem>();
                    var inputItems = input.ServiceItems ?? new List<CreateUpdateServiceItemInServiceDto>();

                    foreach (var existing in currentItems.ToList())
                    {
                        var updateDto = inputItems.FirstOrDefault(i => i.Id == existing.Id);
                        if (updateDto != null)
                        {
                            existing.Identifier = updateDto.Identifier;
                            existing.Brand = updateDto.Brand;
                            existing.Model = updateDto.Model;
                            existing.Type = updateDto.Type;
                            existing.Description = updateDto.Description;
                            existing.NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{updateDto.Identifier} {updateDto.Brand} {updateDto.Model} {updateDto.Type.GetDescription()}");
                        }
                        else
                        {
                            _unitOfWork.GenericRepository<ServiceItem>().Remove(existing);
                        }
                    }

                    var itemsToAdd = inputItems.Where(i => !i.Id.HasValue).ToList();
                    foreach (var item in itemsToAdd)
                    {
                        homestayService.ServiceItems!.Add(new ServiceItem
                        {
                            HomestayServiceId = homestayService.Id,
                            Identifier = item.Identifier,
                            Brand = item.Brand,
                            Model = item.Model,
                            Type = item.Type,
                            Description = item.Description,
                            NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{item.Identifier} {item.Brand} {item.Model} {item.Type.GetDescription()}")
                        });
                    }
                    await _unitOfWork.SaveChangeAsync();
                    _unitOfWork.Commit();

                    var dto = ObjectMapper.Map<HomestayService, HomestayServiceDto>(homestayService);
                    return dto;
                }
                catch (Exception e)
                {
                    _unitOfWork.Rollback();
                    throw new GlobalException(
                        code: BaseErrorCode.UnexpectedError,
                        message: e.Message,
                        statusCode: HttpStatusCode.BadRequest
                    );
                }
            }
        }

        public async Task<PagedResultDto<HomestayServiceDto>> GetPagingWithFilterAsync(HomestayServiceFilterDto filter)
        {
            string searchTerm = !string.IsNullOrEmpty(filter.Text) ? $" {NormalizeString.ConvertNormalizeString(filter.Text.Trim())} " : string.Empty;

            var pagedResults = _unitOfWork.GenericRepository<HomestayService>()
                .GetQueryable()
                .AsNoTracking()
                .WhereIf(filter.IsPrepaid.HasValue, r => r.IsPrepaid == filter.IsPrepaid!.Value)
                .WhereIf(!string.IsNullOrEmpty(filter.Text),
                x => x.NormalizeFullInfo!.Contains(searchTerm) ||
                x.NormalizeFullInfo.Contains(filter.Text!.Trim().ToUpper()) ||
                x.Name!.ToUpper().Contains(filter.Text.Trim().ToUpper()) && !x.DeletedAt.HasValue)
            .OrderByDescending(x => x.CreationTime);

            var data = await pagedResults.GetPaged(filter.CurrentPage, filter.PageSize);
            var result = ObjectMapper.Map<PagedResult<HomestayService>, PagedResultDto<HomestayServiceDto>>(data);
            return result;
        }

        public async Task DeleteAsync(Guid id)
        {
            var item = await _unitOfWork.GenericRepository<HomestayService>().GetAsync(x => x.Id == id && !x.DeletedAt.HasValue);
            if (item == null)
            {
                throw new GlobalException(code: HomestayServiceErrorCode.NotFound,
                        message: L[HomestayServiceErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            if (await HasAnyActiveBookings(id))
            {
                throw new GlobalException(code: HomestayServiceErrorCode.CannotDeleteActiveBooking,
                        message: L[HomestayServiceErrorCode.CannotDeleteActiveBooking],
                        statusCode: HttpStatusCode.BadRequest);
            }
            if (await HasAnyServiceItems(id))
            {
                throw new GlobalException(code: HomestayServiceErrorCode.CannotDeleteHasItems,
                        message: L[HomestayServiceErrorCode.CannotDeleteHasItems],
                        statusCode: HttpStatusCode.BadRequest);
            }
            item.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            _unitOfWork.GenericRepository<HomestayService>().Update(item);
            await _unitOfWork.SaveChangeAsync();
        }
        public async Task<List<HomestayServiceDto>> GetAllAsync()
        {
            var threeMonthsAgo = DateTime.SpecifyKind(DateTime.UtcNow.AddMonths(-3), DateTimeKind.Unspecified);
            var homestayServices = await _unitOfWork.GenericRepository<HomestayService>()
                .GetQueryable()
                .AsNoTracking()
                .Where(x => !x.DeletedAt.HasValue || (x.DeletedAt.HasValue && x.DeletedAt.Value.Date >= threeMonthsAgo.Date))
                .OrderByDescending(r => r.CreationTime)
                .ToListAsync();

            return ObjectMapper.Map<List<HomestayServiceDto>>(homestayServices);
        }

        private async Task<bool> HasAnyActiveBookings(Guid id)
        {
            return await _unitOfWork.GenericRepository<BookingService>()
                .GetQueryable()
                .AsNoTracking()
                .Include(x => x.Booking)
                .AnyAsync(x =>
                    x.ServiceId == id &&
                    x.Booking != null &&
                    (x.Booking.Status == BookingStatuses.Pending || x.Booking.Status == BookingStatuses.Confirmed)
                );
        }
        private async Task<bool> HasAnyServiceItems(Guid id)
        {
            return await _unitOfWork.GenericRepository<ServiceItem>()
                .GetQueryable()
                .AsNoTracking()
                .AnyAsync(x => x.HomestayServiceId == id);
        }

        private void HandleInput(CreateUpdateHomestayServiceDto input)
        {
            input.Name = input.Name!.Trim();
            input.Description = input.Description != null ? input.Description.Trim() : null;
            var inputItems = input.ServiceItems;
            if (inputItems != null && inputItems.Any())
            {
                foreach (var item in inputItems)
                {
                    item.Identifier = item.Identifier != null ? item.Identifier.Trim() : null;
                    item.Brand = item.Brand != null ? item.Brand.Trim() : null;
                    item.Model = item.Model != null ? item.Model.Trim() : null;
                    item.Description = item.Description != null ? item.Description.Trim() : null;
                }
                var duplicated = inputItems
                            .Where(i => i.Type != ServiceItemTypes.Others)
                            .GroupBy(i => i.Identifier!.ToLower())
                            .FirstOrDefault(g => g.Count() > 1);

                if (duplicated != null)
                {
                    throw new GlobalException(
                        code: ServiceItemErrorCode.AlreadyExists,
                        message: L[ServiceItemErrorCode.AlreadyExists],
                        statusCode: HttpStatusCode.BadRequest
                    );
                }
            }
        }

        public async Task<List<ServiceItemDto>> GetServiceItemsByServiceIdAsync(Guid id)
        {
            var service = await _unitOfWork.GenericRepository<HomestayService>().GetAsync(x => x.Id == id);

            if (service == null)
                throw new GlobalException(
                    HomestayServiceErrorCode.NotFound,
                    L[HomestayServiceErrorCode.NotFound],
                    HttpStatusCode.NotFound
                );
            var result = await _unitOfWork.GenericRepository<ServiceItem>()
                            .GetQueryable()
                            .AsNoTracking()
                            .Include(x => x.HomestayService)
                            .Where(x => x.HomestayServiceId == id)
                            .ToListAsync();
            return ObjectMapper.Map<List<ServiceItemDto>>(result);
        }

    }
}
