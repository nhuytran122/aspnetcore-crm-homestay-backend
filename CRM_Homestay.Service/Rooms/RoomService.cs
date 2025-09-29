using AutoMapper;
using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Locations;
using CRM_Homestay.Contract.Rooms;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Helper;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.Rooms;
using CRM_Homestay.Localization;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.RegularExpressions;
using Volo.Abp.DependencyInjection;
using CRM_Homestay.Contract.Uploads;
using CRM_Homestay.Entity.Medias;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Entity.RoomPricings;
using System.Security.Cryptography.X509Certificates;
using CRM_Homestay.Entity.Branches;
using CRM_Homestay.Entity.RoomTypes;
using CRM_Homestay.Entity.RoomAmenities;
using CRM_Homestay.Contract.RoomAmenities;
using CRM_Homestay.Contract.RoomPricings;
using CRM_Homestay.Contract.Medias;
using CRM_Homestay.Entity.BookingRooms;
using CRM_Homestay.Entity.Bookings;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Entity.Amenities;

namespace CRM_Homestay.Service.Rooms
{
    public class RoomService : BaseService, IRoomService, ITransientDependency
    {
        private readonly IUploadService _uploadService;
        public RoomService(IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l, IUploadService uploadService) : base(unitOfWork, mapper, l)
        {
            _uploadService = uploadService;
        }

        public async Task<RoomDto> CreateAsync(CreateRoomDto input)
        {
            var branch = await _unitOfWork.GenericRepository<Branch>().GetAsync(x => x.Id == input.BranchId);
            if (branch == null)
            {
                throw new GlobalException(code: BranchErrorCode.NotFound,
                        message: L[BranchErrorCode.NotFound],
                        statusCode: HttpStatusCode.BadRequest);
            }
            var roomType = await _unitOfWork.GenericRepository<RoomType>().GetAsync(x => x.Id == input.RoomTypeId);
            if (roomType == null)
            {
                throw new GlobalException(code: RoomTypeErrorCode.NotFound,
                        message: L[RoomTypeErrorCode.NotFound],
                        statusCode: HttpStatusCode.BadRequest);
            }
            if (await _unitOfWork.GenericRepository<Room>().AnyAsync(x => x.RoomNumber == input.RoomNumber && x.BranchId == input.BranchId))
            {
                throw new GlobalException(code: RoomErrorCode.AlreadyExists,
                        message: L[RoomErrorCode.AlreadyExists],
                        statusCode: HttpStatusCode.BadRequest);
            }

            var room = ObjectMapper.Map<CreateRoomDto, Room>(input);
            if (input.Images != null)
            {
                List<BaseMedia>? images = null;
                images = await _uploadService.UploadImgs(input.Images, "Media:Rooms");
                await _unitOfWork.GenericRepository<BaseMedia>().AddRangeAsync(images);

                room.Medias = images.Select(img => new MediaRoom
                {
                    MediaId = img.Id,
                    RoomId = room.Id
                }).ToList();
            }

            if (input.RoomAmenities != null && input.RoomAmenities.Any())
            {
                var amenityIds = input.RoomAmenities.Select(a => a.AmenityId).ToList();

                // Check AmenityId có tồn tại trong bảng Amenities không
                var existingAmenities = await _unitOfWork.GenericRepository<Amenity>()
                    .GetQueryable()
                    .Where(x => amenityIds.Contains(x.Id))
                    .Select(x => x.Id)
                    .ToListAsync();

                var invalidAmenityIds = amenityIds.Except(existingAmenities).ToList();
                if (invalidAmenityIds.Any())
                {
                    throw new GlobalException(code: AmenityErrorCode.NotFound,
                            message: L[AmenityErrorCode.NotFound],
                            statusCode: HttpStatusCode.BadRequest);
                }
                room.RoomAmenities = input.RoomAmenities.Select(amenity => new RoomAmenity
                {
                    AmenityId = amenity.AmenityId,
                    Quantity = amenity.Quantity,
                    RoomId = room.Id
                }).ToList();
            }
            await _unitOfWork.GenericRepository<Room>().AddAsync(room);
            await _unitOfWork.SaveChangeAsync();
            var result = ObjectMapper.Map<Room, RoomDto>(room);
            string rootUrl = _uploadService.GetRootUrl();
            result.MediaUrl = room.Medias != null && room.Medias.Any()
                ? rootUrl + room.Medias!.First().Media!.FilePath
                : null;
            result.BranchName = branch.Name;
            result.RoomTypeName = roomType.Name;
            return result;
        }

        public async Task<RoomDetailDto> GetByIdAsync(Guid id)
        {
            var room = await _unitOfWork.GenericRepository<Room>().GetQueryable().AsNoTracking()
            .Include(x => x.Medias!).ThenInclude(y => y.Media)
            .Include(x => x.Branch)
            .Include(x => x.RoomType).ThenInclude(y => y!.RoomPricings)
            .Include(x => x.RoomAmenities!).ThenInclude(y => y.Amenity)
            .FirstOrDefaultAsync(x => x.Id == id && !x.DeletedAt.HasValue);

            if (room == null)
            {
                throw new GlobalException(
                    code: RoomErrorCode.NotFound,
                    message: L[RoomErrorCode.NotFound],
                    statusCode: HttpStatusCode.NotFound
                );
            }

            string rootUrl = _uploadService.GetRootUrl();

            var result = new RoomDetailDto
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                IsActive = room.IsActive,
                BranchId = room.BranchId,
                BranchName = room.Branch?.Name,
                RoomTypeId = room.RoomTypeId,
                RoomTypeName = room.RoomType?.Name,

                RoomAmenities = ObjectMapper.Map<List<RoomAmenityDto>>(room.RoomAmenities),

                Prices = ObjectMapper.Map<List<RoomPricingDto>>(room.RoomType?.RoomPricings),

                Images = room.Medias?.Select(m => new MediaInfoDto
                {
                    Id = m.Id,
                    ImagePath = m.Media != null ? rootUrl + m.Media.FilePath : null
                }).ToList()
            };
            return result;
        }

        public async Task<RoomDto> UpdateAsync(Guid id, UpdateRoomDto input)
        {
            var room = await _unitOfWork.GenericRepository<Room>().GetQueryable()
                .Include(x => x.Medias!).ThenInclude(y => y.Media)
                .Include(x => x.RoomAmenities)
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeletedAt.HasValue);

            if (room == null)
                throw new GlobalException(code: RoomErrorCode.NotFound,
                    message: L[RoomErrorCode.NotFound],
                    statusCode: HttpStatusCode.NotFound);

            var branch = await _unitOfWork.GenericRepository<Branch>().GetAsync(x => x.Id == input.BranchId && !x.DeletedAt.HasValue);
            if (branch == null)
                throw new GlobalException(code: BranchErrorCode.NotFound,
                    message: L[BranchErrorCode.NotFound],
                    statusCode: HttpStatusCode.BadRequest);

            var roomType = await _unitOfWork.GenericRepository<RoomType>().GetAsync(x => x.Id == input.RoomTypeId && !x.DeletedAt.HasValue);
            if (roomType == null)
                throw new GlobalException(code: RoomTypeErrorCode.NotFound,
                    message: L[RoomTypeErrorCode.NotFound],
                    statusCode: HttpStatusCode.BadRequest);

            if (await _unitOfWork.GenericRepository<Room>()
                .AnyAsync(x => x.Id != id && x.RoomNumber == input.RoomNumber && x.BranchId == input.BranchId))
                throw new GlobalException(code: RoomErrorCode.AlreadyExists,
                    message: L[RoomErrorCode.AlreadyExists],
                    statusCode: HttpStatusCode.BadRequest);

            var currentRoomAmenities = room.RoomAmenities ??= new List<RoomAmenity>();
            ObjectMapper.Map(input, room);
            using (_unitOfWork.BeginTransaction())
                try
                {
                    if (input.RoomAmenities != null && input.RoomAmenities.Any())
                    {
                        var allAmenityIds = input.RoomAmenities
                            .Where(a => a.AmenityId != Guid.Empty)
                            .Select(a => a.AmenityId)
                            .Distinct()
                            .ToList();

                        // Check tồn tại trong DB cho AmenityId
                        var existingAmenities = await _unitOfWork.GenericRepository<Amenity>()
                            .GetQueryable()
                            .Where(x => allAmenityIds.Contains(x.Id))
                            .Select(x => x.Id)
                            .ToListAsync();

                        var invalidAmenityIds = allAmenityIds.Except(existingAmenities).ToList();
                        if (invalidAmenityIds.Any())
                        {
                            throw new GlobalException(
                                code: AmenityErrorCode.NotFound,
                                message: L[AmenityErrorCode.NotFound],
                                statusCode: HttpStatusCode.BadRequest
                            );
                        }

                        // Tách input: Add mới và Update
                        var inputAmenitiesToAdd = input.RoomAmenities
                            .Where(x => !x.Id.HasValue && x.AmenityId != Guid.Empty)
                            .ToList();

                        var inputRoomAmenitiesToUpdate = input.RoomAmenities
                            .Where(x => x.Id.HasValue)
                            .ToList();

                        // Xóa RoomAmenity trong DB nhưng không có trong input
                        var inputRoomAmenityIds = inputRoomAmenitiesToUpdate.Select(x => x.Id!.Value).ToList();
                        var roomAmenitiesToDelete = currentRoomAmenities
                            .Where(x => !inputRoomAmenityIds.Contains(x.Id))
                            .ToList();

                        // Xóa RoomAmenity không có trong input
                        if (roomAmenitiesToDelete.Any())
                        {
                            _unitOfWork.GenericRepository<RoomAmenity>().RemoveRange(roomAmenitiesToDelete);
                        }

                        // Update chỉ Quantity
                        foreach (var item in inputRoomAmenitiesToUpdate)
                        {
                            var roomAmenity = currentRoomAmenities.FirstOrDefault(x => x.Id == item.Id);
                            if (roomAmenity != null && item.Quantity != roomAmenity.Quantity)
                            {
                                roomAmenity.Quantity = item.Quantity;
                            }
                        }

                        // Thêm mới
                        foreach (var item in inputAmenitiesToAdd)
                        {
                            await _unitOfWork.GenericRepository<RoomAmenity>().AddAsync(new RoomAmenity
                            {
                                AmenityId = item.AmenityId,
                                Quantity = item.Quantity,
                                RoomId = room.Id
                            });
                        }
                    }
                    //K truyền => k đổi
                    // else
                    // {
                    //     if(currentRoomAmenities != null)
                    //         _unitOfWork.GenericRepository<RoomAmenity>().RemoveRange(currentRoomAmenities);
                    // }

                    if (input.Images != null && input.Images.Any())
                    {
                        if (room.Medias != null && room.Medias.Any())
                        {
                            var oldFilePaths = room.Medias
                                                    .Select(mr => mr.Media != null ? mr.Media.FilePath : null)
                                                    .Where(fp => fp != null)
                                                    .ToList();

                            _uploadService.DeleteImages(oldFilePaths!);

                            var oldBaseMedia = room.Medias.Select(mr => mr.Media!).ToList();
                            _unitOfWork.GenericRepository<BaseMedia>().RemoveRange(oldBaseMedia);
                        }

                        var newBaseMedias = await _uploadService.UploadImgs(input.Images, "Media:Rooms");
                        await _unitOfWork.GenericRepository<BaseMedia>().AddRangeAsync(newBaseMedias);
                        room.Medias = newBaseMedias.Select(m => new MediaRoom
                        {
                            MediaId = m.Id,
                            RoomId = id,
                            Media = m
                        }).ToList();
                    }

                    await _unitOfWork.SaveChangeAsync();
                    _unitOfWork.Commit();

                    var dto = ObjectMapper.Map<Room, RoomDto>(room);

                    string rootUrl = _uploadService.GetRootUrl();
                    var firstMedia = room.Medias?.FirstOrDefault()?.Media;
                    dto.MediaUrl = firstMedia != null ? rootUrl + firstMedia.FilePath : null;
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
        public async Task<PagedResultDto<RoomDto>> GetPagingWithFilterAsync(RoomFilterDto filter)
        {
            string rootUrl = _uploadService.GetRootUrl();

            var query = _unitOfWork.GenericRepository<Room>()
                .GetQueryable()
                .AsNoTracking()
                .Include(r => r.Medias!)
                    .ThenInclude(mr => mr.Media)
                .Include(r => r.RoomType)
                    .ThenInclude(rt => rt!.RoomPricings)
                .Include(r => r.Branch)
                .Include(r => r.RoomAmenities)
                .Where(r => !r.DeletedAt.HasValue && r.IsActive)
                .WhereIf(filter.BranchId.HasValue, r => r.BranchId == filter.BranchId!.Value)
                .WhereIf(filter.RoomTypeId.HasValue, r => r.RoomTypeId == filter.RoomTypeId!.Value)
                .WhereIf(filter.GuestCounts.HasValue, r => r.RoomType != null && r.RoomType.MaxGuests >= filter.GuestCounts!.Value)
                .WhereIf(filter.MinPrice.HasValue, r => r.RoomType != null &&
                    r.RoomType.RoomPricings != null && r.RoomType.RoomPricings.Any(p => p.BasePrice >= filter.MinPrice!.Value))
                .WhereIf(filter.MaxPrice.HasValue, r => r.RoomType != null &&
                    r.RoomType.RoomPricings != null && r.RoomType.RoomPricings.Any(p => p.BasePrice <= filter.MaxPrice!.Value))
                .WhereIf(filter.AmenityIds != null && filter.AmenityIds.Any(),
                    r => filter.AmenityIds!.All(id => r.RoomAmenities != null && r.RoomAmenities.Any(ra => ra.AmenityId == id)))
                .WhereIf(filter.CheckInDate.HasValue && filter.CheckOutDate.HasValue, r =>
                    r.RoomUsages == null || !r.RoomUsages.Any(u =>
                        u.StartAt < filter.CheckOutDate!.Value && u.EndAt > filter.CheckInDate!.Value
                    )
                )
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    IsActive = r.IsActive,
                    BranchId = r.BranchId,
                    BranchName = r.Branch != null ? r.Branch.Name : null,
                    RoomTypeId = r.RoomTypeId,
                    RoomTypeName = r.RoomType != null ? r.RoomType.Name : null,
                    MediaUrl = r.Medias != null && r.Medias.Any() && r.Medias.First().Media != null
                        ? rootUrl + r.Medias.First().Media!.FilePath
                        : null
                });

            var data = await query.GetPaged(filter.CurrentPage, filter.PageSize);
            var result = ObjectMapper.Map<PagedResult<RoomDto>, PagedResultDto<RoomDto>>(data);
            return result;
        }

        public async Task DeleteAsync(Guid id)
        {
            var room = await _unitOfWork.GenericRepository<Room>().GetAsync(x => x.Id == id && !x.DeletedAt.HasValue);
            if (room == null)
            {
                throw new GlobalException(code: RoomErrorCode.NotFound,
                        message: L[RoomErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            if (await HasAnyActiveBookings(id))
            {
                throw new GlobalException(code: RoomErrorCode.NotAllowedDelete,
                        message: L[RoomErrorCode.NotAllowedDelete],
                        statusCode: HttpStatusCode.BadRequest);
            }
            room.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            _unitOfWork.GenericRepository<Room>().Update(room);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<List<RoomDto>> GetAllAsync()
        {
            string rootUrl = _uploadService.GetRootUrl();

            var rooms = await _unitOfWork.GenericRepository<Room>()
                .GetQueryable()
                .AsNoTracking()
                .Include(x => x.RoomType)
                .Include(x => x.Branch)
                .Include(r => r.Medias!)
                    .ThenInclude(mr => mr.Media)
                .Where(r => !r.DeletedAt.HasValue)
                .OrderByDescending(r => r.CreationTime)
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    IsActive = r.IsActive,
                    BranchId = r.BranchId,
                    BranchName = r.Branch != null ? r.Branch.Name : null,
                    RoomTypeId = r.RoomTypeId,
                    RoomTypeName = r.RoomType != null ? r.RoomType.Name : null,
                    MediaUrl = r.Medias != null && r.Medias.Any() && r.Medias.First().Media != null
                                ? rootUrl + r.Medias!.First().Media!.FilePath
                                : null,
                })
                .ToListAsync();
            return rooms;
        }

        private async Task<bool> HasAnyActiveBookings(Guid roomId)
        {
            return await _unitOfWork.GenericRepository<BookingRoom>()
                .GetQueryable()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.RoomId == roomId &&
                    x.Booking != null &&
                    (x.Booking.Status == BookingStatuses.Pending || x.Booking.Status == BookingStatuses.Confirmed)
                );
        }

        public async Task<List<RoomDto>> GetAllActiveAsync()
        {
            string rootUrl = _uploadService.GetRootUrl();
            var result = await _unitOfWork.GenericRepository<Room>()
                .GetQueryable()
                .AsNoTracking()
                .Include(x => x.RoomType)
                .Include(x => x.Branch)
                .Include(x => x.Medias!)
                    .ThenInclude(y => y.Media)
                .Where(x => x.IsActive && !x.DeletedAt.HasValue)
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    IsActive = r.IsActive,
                    BranchId = r.BranchId,
                    BranchName = r.Branch != null ? r.Branch.Name : null,
                    RoomTypeId = r.RoomTypeId,
                    RoomTypeName = r.RoomType != null ? r.RoomType.Name : null,
                    MediaUrl = r.Medias != null && r.Medias.Any() && r.Medias.First().Media != null
                                ? rootUrl + r.Medias!.First().Media!.FilePath
                                : null,
                })
                .ToListAsync();
            return result;
        }

        public async Task<List<RoomDto>> GetByBranchAsync(Guid branchId)
        {
            var rooms = await _unitOfWork.GenericRepository<Room>()
                .GetQueryable()
                .AsNoTracking()
                .Include(x => x.Branch)
                .Include(x => x.RoomType)
                .WhereIf(branchId != Guid.Empty, x => x.RoomTypeId == branchId)
                .Where(x => x.DeletedAt.HasValue)
                .ToListAsync();

            return ObjectMapper.Map<List<Room>, List<RoomDto>>(rooms);
        }

        public async Task<List<RoomDto>> GetByRoomTypeAsync(Guid roomTypeId)
        {
            var rooms = await _unitOfWork.GenericRepository<Room>()
                .GetQueryable()
                .AsNoTracking()
                .Include(x => x.Branch)
                .Include(x => x.RoomType)
                .WhereIf(roomTypeId != Guid.Empty, x => x.RoomTypeId == roomTypeId)
                .Where(x => x.DeletedAt.HasValue)
                .ToListAsync();

            return ObjectMapper.Map<List<Room>, List<RoomDto>>(rooms);
        }

        public async Task<List<RoomDto>> GetByBranchAndRoomTypeAsync(RoomFilterDto input)
        {
            var rooms = await _unitOfWork.GenericRepository<Room>()
                .GetQueryable()
                .AsNoTracking()
                .Include(x => x.Branch)
                .Include(x => x.RoomType)
                .WhereIf(input.BranchId != null, x => x.BranchId == input.BranchId)
                .WhereIf(input.RoomTypeId != null, x => x.RoomTypeId == input.RoomTypeId)
                .Where(x => x.DeletedAt.HasValue)
                .ToListAsync();

            return ObjectMapper.Map<List<Room>, List<RoomDto>>(rooms);
        }
    }
}
