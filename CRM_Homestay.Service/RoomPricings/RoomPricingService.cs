using AutoMapper;
using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Locations;
using CRM_Homestay.Contract.RoomPricings;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Helper;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.RoomPricings;
using CRM_Homestay.Localization;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Volo.Abp.DependencyInjection;
using CRM_Homestay.Entity.Medias;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Entity.Rooms;
using CRM_Homestay.Entity.RoomTypes;

namespace CRM_Homestay.Service.RoomPricings
{
    public class RoomPricingService : BaseService, IRoomPricingService, ITransientDependency
    {
        public RoomPricingService(IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l) : base(unitOfWork, mapper, l)
        {
        }

        public async Task<RoomPricingDto> CreateAsync(CreateRoomPricingDto input)
        {
            HandleInput(input);
            var roomType = await _unitOfWork.GenericRepository<RoomType>().GetAsync(x => x.Id == input.RoomTypeId && !x.DeletedAt.HasValue);
            if (roomType == null)
            {
                throw new GlobalException(code: RoomTypeErrorCode.NotFound,
                        message: L[RoomTypeErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            if (!input.IsDefault)
            {
                var hasOverlap = await _unitOfWork.GenericRepository<RoomPricing>()
                    .AnyAsync(x =>
                        x.RoomTypeId == input.RoomTypeId &&
                        !x.IsDefault &&
                        !x.DeletedAt.HasValue &&
                        x.StartAt <= input.EndAt &&
                        input.StartAt <= x.EndAt
                    );

                if (hasOverlap)
                {
                    throw new GlobalException(
                        code: RoomPricingErrorCode.OverlappingDateRange,
                        message: L[RoomPricingErrorCode.OverlappingDateRange],
                        statusCode: HttpStatusCode.BadRequest
                    );
                }
            }
            var existingDefault = await _unitOfWork.GenericRepository<RoomPricing>()
                                    .GetQueryable()
                                    .AsNoTracking()
                                    .Where(x => x.RoomTypeId == input.RoomTypeId && x.IsDefault && !x.DeletedAt.HasValue)
                                    .FirstOrDefaultAsync();
            if (input.IsDefault)
            {
                // Có bản ghi IsDefault, set về false
                if (existingDefault != null)
                {
                    existingDefault.IsDefault = false;
                    _unitOfWork.GenericRepository<RoomPricing>().Update(existingDefault);
                }
            }
            var roomPricing = ObjectMapper.Map<CreateRoomPricingDto, RoomPricing>(input);
            await _unitOfWork.GenericRepository<RoomPricing>().AddAsync(roomPricing);

            await _unitOfWork.SaveChangeAsync();
            var result = ObjectMapper.Map<RoomPricing, RoomPricingDto>(roomPricing);
            return result;
        }

        public async Task<RoomPricingDto> GetByIdAsync(Guid id)
        {
            var roomPricing = await _unitOfWork.GenericRepository<RoomPricing>()
                                                        .GetQueryable()
                                                        .AsNoTracking()
                                                        .Include(x => x.RoomType)
                                                        .FirstOrDefaultAsync(x => x.Id == id && !x.DeletedAt.HasValue);
            if (roomPricing == null)
            {
                throw new GlobalException(code: RoomPricingErrorCode.NotFound,
                        message: L[RoomPricingErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            return ObjectMapper.Map<RoomPricing, RoomPricingDto>(roomPricing);
        }

        public async Task<RoomPricingDto> UpdateAsync(Guid id, UpdateRoomPricingDto input)
        {
            HandleInput(input);

            var roomPricing = await _unitOfWork.GenericRepository<RoomPricing>()
                .GetQueryable()
                .AsNoTracking()
                .Include(x => x.RoomType)
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeletedAt.HasValue);

            if (roomPricing == null)
            {
                throw new GlobalException(
                    code: RoomPricingErrorCode.NotFound,
                    message: L[RoomPricingErrorCode.NotFound],
                    statusCode: HttpStatusCode.NotFound
                );
            }

            // non-default -> check overlap
            if (!input.IsDefault)
            {
                var hasOverlap = await _unitOfWork.GenericRepository<RoomPricing>()
                    .AnyAsync(x =>
                        x.RoomTypeId == roomPricing.RoomTypeId &&
                        x.Id != id &&
                        !x.IsDefault &&
                        !x.DeletedAt.HasValue &&
                        x.StartAt <= input.EndAt &&
                        input.StartAt <= x.EndAt
                    );

                if (hasOverlap)
                {
                    throw new GlobalException(
                        code: RoomPricingErrorCode.OverlappingDateRange,
                        message: L[RoomPricingErrorCode.OverlappingDateRange],
                        statusCode: HttpStatusCode.BadRequest
                    );
                }
            }

            // Đang là default-> muốn bỏ default -> chặn
            if (roomPricing.IsDefault && !input.IsDefault)
            {
                var isOnlyDefault = await _unitOfWork.GenericRepository<RoomPricing>()
                    .GetQueryable()
                    .Where(x => x.RoomTypeId == roomPricing.RoomTypeId && x.IsDefault && !x.DeletedAt.HasValue)
                    .CountAsync() == 1;

                if (isOnlyDefault)
                {
                    throw new GlobalException(
                        code: RoomPricingErrorCode.CannotRemoveOnlyDefaultPricing,
                        message: L[RoomPricingErrorCode.CannotRemoveOnlyDefaultPricing],
                        statusCode: HttpStatusCode.BadRequest
                    );
                }
            }

            // Input set IsDefault = true -> bỏ default cũ
            if (input.IsDefault)
            {
                var existingDefault = await _unitOfWork.GenericRepository<RoomPricing>()
                    .GetQueryable()
                    .Where(x => x.RoomTypeId == roomPricing.RoomTypeId &&
                                x.Id != id &&
                                x.IsDefault &&
                                !x.DeletedAt.HasValue)
                    .FirstOrDefaultAsync();

                if (existingDefault != null)
                {
                    existingDefault.IsDefault = false;
                    _unitOfWork.GenericRepository<RoomPricing>().Update(existingDefault);
                }
            }

            var updatedItem = ObjectMapper.Map(input, roomPricing);
            updatedItem.RoomTypeId = roomPricing.RoomTypeId;

            _unitOfWork.GenericRepository<RoomPricing>().Update(updatedItem);
            await _unitOfWork.SaveChangeAsync();

            return ObjectMapper.Map<RoomPricing, RoomPricingDto>(updatedItem);
        }
        private void HandleInput(CreateRoomPricingDto input)
        {
            input.Description = input.Description?.Trim();
            if (input.StartAt < DateTime.UtcNow)
            {
                throw new GlobalException(code: DateErrorCode.StartDateMustBeFuture,
                        message: L[DateErrorCode.StartDateMustBeFuture],
                        statusCode: HttpStatusCode.BadRequest);
            }
        }
        private void HandleInput(UpdateRoomPricingDto input)
        {
            input.Description = input.Description?.Trim();
        }

        public async Task DeleteAsync(Guid id)
        {
            var roomPricing = await _unitOfWork.GenericRepository<RoomPricing>().GetAsync(x => x.Id == id && !x.DeletedAt.HasValue);
            if (roomPricing == null)
            {
                throw new GlobalException(code: RoomPricingErrorCode.NotFound,
                        message: L[RoomPricingErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            if (roomPricing.IsDefault)
            {
                throw new GlobalException(code: RoomPricingErrorCode.NotAllowedDelete,
                        message: L[RoomPricingErrorCode.NotAllowedDelete],
                        statusCode: HttpStatusCode.BadRequest);
            }
            roomPricing.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            _unitOfWork.GenericRepository<RoomPricing>().Update(roomPricing);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<List<RoomPricingDto>> GetPricingByRoomTypeId(Guid typeId)
        {
            var result = await _unitOfWork.GenericRepository<RoomPricing>()
                .GetQueryable()
                .AsNoTracking()
                .Include(x => x.RoomType)
                .Where(x => x.RoomTypeId == typeId && !x.DeletedAt.HasValue)
                .OrderByDescending(x => x.CreationTime)
                .ToListAsync();

            return ObjectMapper.Map<List<RoomPricing>, List<RoomPricingDto>>(result);
        }

        public async Task<List<RoomPricingDto>> GetAllAsync()
        {
            var threeMonthsAgo = DateTime.SpecifyKind(DateTime.UtcNow.AddMonths(-3), DateTimeKind.Unspecified);

            var result = await _unitOfWork.GenericRepository<RoomPricing>()
                .GetQueryable()
                .AsNoTracking()
                .Include(x => x.RoomType)
                .Where(x => !x.DeletedAt.HasValue || (x.DeletedAt.HasValue && x.DeletedAt.Value.Date >= threeMonthsAgo.Date))
                .OrderByDescending(x => x.RoomTypeId)
                .ToListAsync();

            return ObjectMapper.Map<List<RoomPricing>, List<RoomPricingDto>>(result);
        }
    }
}
