using AutoMapper;
using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Locations;
using CRM_Homestay.Contract.RoomTypes;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Helper;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.RoomTypes;
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
using CRM_Homestay.Entity.Rooms;

namespace CRM_Homestay.Service.RoomTypes
{
    public class RoomTypeService : BaseService, IRoomTypeService, ITransientDependency
    {
        private readonly IUploadService _uploadService;
        public RoomTypeService(IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l, IUploadService uploadService) : base(unitOfWork, mapper, l)
        {
            _uploadService = uploadService;
        }

        public async Task<RoomTypeDto> CreateAsync(CreateUpdateRoomTypeDto input)
        {
            HandleInput(input);
            if (await _unitOfWork.GenericRepository<RoomType>().AnyAsync(x => x.Name.ToLower() == input.Name.ToLower()))
            {
                throw new GlobalException(code: RoomTypeErrorCode.AlreadyExists,
                        message: L[RoomTypeErrorCode.AlreadyExists],
                        statusCode: HttpStatusCode.BadRequest);
            }

            var roomType = ObjectMapper.Map<CreateUpdateRoomTypeDto, RoomType>(input);
            roomType.NormalizeFullInfo = NormalizeString.ConvertNormalizeString(input.Name!);
            if (input.Image != null)
            {
                BaseMedia image = await _uploadService.UploadImg(input.Image, "Media:RoomTypes");
                await _unitOfWork.GenericRepository<BaseMedia>().AddAsync(image);
                roomType.Media = image;
            }
            var pricing = new RoomPricing()
            {
                BaseDuration = input.BaseDuration,
                BasePrice = input.BasePrice,
                ExtraHourPrice = input.ExtraHourPrice,
                OvernightPrice = input.OvernightPrice,
                DailyPrice = input.DailyPrice,
                IsDefault = true,
            };
            roomType.RoomPricings = new List<RoomPricing>() { pricing };
            await _unitOfWork.GenericRepository<RoomType>().AddAsync(roomType);

            await _unitOfWork.SaveChangeAsync();
            var result = ObjectMapper.Map<RoomType, RoomTypeDto>(roomType);
            string rootUrl = _uploadService.GetRootUrl();
            result.MediaUrl = roomType.Media != null ? rootUrl + roomType.Media.FilePath : null;

            MapDefaultPricing(roomType, result);

            return result;
        }

        public async Task<RoomTypeDto> GetByIdAsync(Guid id)
        {
            var roomType = await _unitOfWork.GenericRepository<RoomType>()
                                                        .GetQueryable()
                                                        .AsNoTracking()
                                                        .Include(x => x.Media)
                                                        .Include(x => x.RoomPricings)
                                                        .FirstOrDefaultAsync(x => x.Id == id && !x.DeletedAt.HasValue);
            if (roomType == null)
            {
                throw new GlobalException(code: RoomTypeErrorCode.NotFound,
                        message: L[RoomTypeErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            string rootUrl = _uploadService.GetRootUrl();
            var roomTypeDto = ObjectMapper.Map<RoomType, RoomTypeDto>(roomType);
            roomTypeDto.MediaUrl = roomType.Media != null ? rootUrl + roomType.Media.FilePath : null;

            MapDefaultPricing(roomType, roomTypeDto);
            return roomTypeDto;
        }

        public async Task<RoomTypeDto> UpdateAsync(Guid id, CreateUpdateRoomTypeDto input)
        {
            var roomType = await _unitOfWork.GenericRepository<RoomType>()
                                                        .GetQueryable()
                                                        .AsNoTracking()
                                                        .Include(x => x.Media)
                                                        .Include(x => x.RoomPricings)
                                                        .FirstOrDefaultAsync(x => x.Id == id && !x.DeletedAt.HasValue);
            if (roomType == null)
            {
                throw new GlobalException(code: RoomTypeErrorCode.NotFound,
                        message: L[RoomTypeErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            var result = await _unitOfWork.GenericRepository<RoomType>().GetListAsync(x => x.Id != id && x.Name.ToLower() == input.Name.ToLower());
            if (result.Count > 0)
            {
                throw new GlobalException(code: RoomTypeErrorCode.AlreadyExists,
                        message: L[RoomTypeErrorCode.AlreadyExists],
                        statusCode: HttpStatusCode.BadRequest);
            }

            var newRoomTypeInfo = ObjectMapper.Map(input, roomType);
            newRoomTypeInfo.NormalizeFullInfo = NormalizeString.ConvertNormalizeString(input.Name);
            if (input.Image != null)
            {
                if (roomType.Media != null)
                {
                    _uploadService.DeleteImage(roomType.Media.FilePath);
                    _unitOfWork.GenericRepository<BaseMedia>().Remove(roomType.Media);
                }

                BaseMedia image = await _uploadService.UploadImg(input.Image, "Media:RoomTypes");
                await _unitOfWork.GenericRepository<BaseMedia>().AddAsync(image);
                newRoomTypeInfo.Media = image;
            }
            else
            {
                newRoomTypeInfo.Media = roomType.Media;
            }
            var pricing = roomType.RoomPricings!.FirstOrDefault(x => x.IsDefault);
            if (pricing != null)
            {
                pricing.BaseDuration = input.BaseDuration;
                pricing.BasePrice = input.BasePrice;
                pricing.ExtraHourPrice = input.ExtraHourPrice;
                pricing.OvernightPrice = input.OvernightPrice;
                pricing.DailyPrice = input.DailyPrice;
            }

            _unitOfWork.GenericRepository<RoomType>().Update(newRoomTypeInfo);
            await _unitOfWork.SaveChangeAsync();

            var dto = ObjectMapper.Map<RoomType, RoomTypeDto>(roomType);
            string rootUrl = _uploadService.GetRootUrl();
            dto.MediaUrl = roomType.Media != null ? rootUrl + roomType.Media.FilePath : null;

            MapDefaultPricing(roomType, dto);
            return dto;
        }

        private void HandleInput(CreateUpdateRoomTypeDto input)
        {
            input.Name = input.Name.Trim();
            input.Name = Regex.Replace(input.Name, @"\s+", " ");
            input.Description = input.Description?.Trim();
        }

        public async Task<PagedResultDto<RoomTypeDto>> GetPagingWithFilterAsync(RoomTypeFilterDto input)
        {
            var searchTerm = !string.IsNullOrEmpty(input.Text) ? $" {NormalizeString.ConvertNormalizeString(input.Text)} " : string.Empty;
            string rootUrl = _uploadService.GetRootUrl();
            var query = _unitOfWork.GenericRepository<RoomType>()
                                                        .GetQueryable()
                                                        .Include(x => x.Media)
                                                        .Include(x => x.RoomPricings)
                                                        .OrderByDescending(x => x.CreationTime)
                                                        .Where(x => !x.DeletedAt.HasValue)
                                                        .WhereIf(!string.IsNullOrEmpty(input.Text),
                                                            x => (" " + x.NormalizeFullInfo + " ").Contains(searchTerm))
                                                        .WhereIf(input.GuestCounts != null, x => x.MaxGuests >= input.GuestCounts)
                                                        .WhereIf(input.StartDate != null, x => x.CreationTime.AddHours(7).Date >= input.StartDate!.Value.Date)
                                                        .WhereIf(input.EndDate != null, x => x.CreationTime.AddHours(7).Date <= input.EndDate!.Value.Date)
                                                        .Select(x => new
                                                        {
                                                            RoomType = x,
                                                            DefaultPricing = x.RoomPricings!.FirstOrDefault(p => p.IsDefault)
                                                        })
                                                        .Select(a => new RoomTypeDto
                                                        {
                                                            Id = a.RoomType.Id,
                                                            Name = a.RoomType.Name,
                                                            CreationTime = a.RoomType.CreationTime,
                                                            MediaUrl = a.RoomType.Media != null ? rootUrl + a.RoomType.Media.FilePath : null,

                                                            BaseDuration = a.DefaultPricing!.BaseDuration,
                                                            BasePrice = a.DefaultPricing.BasePrice,
                                                            ExtraHourPrice = a.DefaultPricing.ExtraHourPrice,
                                                            OvernightPrice = a.DefaultPricing.OvernightPrice,
                                                            DailyPrice = a.DefaultPricing.DailyPrice
                                                        });

            var data = await query.GetPaged(input.CurrentPage, input.PageSize);
            var result = ObjectMapper.Map<PagedResult<RoomTypeDto>, PagedResultDto<RoomTypeDto>>(data);

            return result;
        }

        public async Task DeleteAsync(Guid id)
        {
            var roomType = await _unitOfWork.GenericRepository<RoomType>().GetAsync(x => x.Id == id && !x.DeletedAt.HasValue);
            if (roomType == null)
            {
                throw new GlobalException(code: RoomTypeErrorCode.NotFound,
                        message: L[RoomTypeErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            if (await HasAnyRooms(id))
            {
                throw new GlobalException(code: RoomTypeErrorCode.NotAllowedDelete,
                        message: L[RoomTypeErrorCode.NotAllowedDelete],
                        statusCode: HttpStatusCode.BadRequest);
            }
            roomType.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            _unitOfWork.GenericRepository<RoomType>().Update(roomType);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<List<RoomTypeDto>> GetAllAsync()
        {
            var threeMonthsAgo = DateTime.SpecifyKind(DateTime.UtcNow.AddMonths(-3), DateTimeKind.Unspecified);
            string rootUrl = _uploadService.GetRootUrl();

            var roomTypes = await _unitOfWork.GenericRepository<RoomType>()
                .GetQueryable()
                .Include(x => x.Media)
                .Where(x => !x.DeletedAt.HasValue || (x.DeletedAt.HasValue && x.DeletedAt.Value.Date >= threeMonthsAgo.Date))
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new
                {
                    RoomType = x,
                    DefaultPricing = x.RoomPricings!.FirstOrDefault(p => p.IsDefault)
                })
                .Select(a => new RoomTypeDto
                {
                    Id = a.RoomType.Id,
                    Name = a.RoomType.Name,
                    CreationTime = a.RoomType.CreationTime,
                    MaxGuests = a.RoomType.MaxGuests,
                    MediaUrl = a.RoomType.Media != null ? rootUrl + a.RoomType.Media.FilePath : null,

                    BaseDuration = a.DefaultPricing!.BaseDuration,
                    BasePrice = a.DefaultPricing.BasePrice,
                    ExtraHourPrice = a.DefaultPricing.ExtraHourPrice,
                    OvernightPrice = a.DefaultPricing.OvernightPrice,
                    DailyPrice = a.DefaultPricing.DailyPrice
                })
                .ToListAsync();
            return roomTypes;
        }

        private async Task<bool> HasAnyRooms(Guid id)
        {
            if (await _unitOfWork.GenericRepository<Room>().AnyAsync(x => x.RoomTypeId == id && !x.DeletedAt.HasValue))
                return true;
            return false;
        }
        private void MapDefaultPricing(RoomType roomType, RoomTypeDto dto)
        {
            var defaultPricing = roomType.RoomPricings?.FirstOrDefault(x => x.IsDefault);
            if (defaultPricing != null)
            {
                dto.BaseDuration = defaultPricing.BaseDuration;
                dto.BasePrice = defaultPricing.BasePrice;
                dto.ExtraHourPrice = defaultPricing.ExtraHourPrice;
                dto.OvernightPrice = defaultPricing.OvernightPrice;
                dto.DailyPrice = defaultPricing.DailyPrice;
            }
        }
    }
}
