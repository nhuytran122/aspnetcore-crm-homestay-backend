using AutoMapper;
using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Helper;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Localization;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.RegularExpressions;
using Volo.Abp.DependencyInjection;
using CRM_Homestay.Contract.Amenities;
using CRM_Homestay.Entity.Amenities;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Extensions;
using CRM_Homestay.Entity.RoomAmenities;
using CRM_Homestay.Contract.RoomAmenities;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Service.Amenities
{
    public class AmenityService : BaseService, IAmenityService, ITransientDependency
    {
        public AmenityService(IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l) : base(unitOfWork, mapper, l)
        {
        }

        public async Task<AmenityDto> CreateAsync(CreateUpdateAmenityDto input)
        {
            HandleInput(input);
            if (await _unitOfWork.GenericRepository<Amenity>().AnyAsync(x => x.Name!.ToLower() == input.Name!.ToLower()))
            {
                throw new GlobalException(code: AmenityErrorCode.AlreadyExists,
                        message: L[AmenityErrorCode.AlreadyExists],
                        statusCode: HttpStatusCode.BadRequest);
            }

            var amenity = ObjectMapper.Map<CreateUpdateAmenityDto, Amenity>(input);

            amenity.NormalizeFullInfo = NormalizeString.ConvertNormalizeString(input.Name!);

            await _unitOfWork.GenericRepository<Amenity>().AddAsync(amenity);
            await _unitOfWork.SaveChangeAsync();
            return ObjectMapper.Map<Amenity, AmenityDto>(amenity);
        }

        public async Task<AmenityDto> GetByIdAsync(Guid id)
        {
            var amenity = await _unitOfWork.GenericRepository<Amenity>().GetAsync(x => x.Id == id);
            if (amenity == null)
            {
                throw new GlobalException(code: AmenityErrorCode.NotFound,
                        message: L[AmenityErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            return ObjectMapper.Map<Amenity, AmenityDto>(amenity);
        }

        public async Task<AmenityDto> UpdateAsync(Guid id, CreateUpdateAmenityDto input)
        {
            var amenity = await _unitOfWork.GenericRepository<Amenity>().GetAsync(x => x.Id == id);
            if (amenity == null)
            {
                throw new GlobalException(code: AmenityErrorCode.NotFound,
                        message: L[AmenityErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            var result = await _unitOfWork.GenericRepository<Amenity>().GetListAsync(x => x.Id != id && x.Name.ToLower() == input.Name!.ToLower());
            if (result.Count > 0)
            {
                throw new GlobalException(code: AmenityErrorCode.AlreadyExists,
                        message: L[AmenityErrorCode.AlreadyExists],
                        statusCode: HttpStatusCode.BadRequest);
            }
            var newAmenityInfo = ObjectMapper.Map(input, amenity);
            _unitOfWork.GenericRepository<Amenity>().Update(newAmenityInfo);
            await _unitOfWork.SaveChangeAsync();

            return ObjectMapper.Map<Amenity, AmenityDto>(newAmenityInfo);
        }

        private void HandleInput(CreateUpdateAmenityDto input)
        {
            input.Name = input.Name!.Trim();
            input.Name = Regex.Replace(input.Name, @"\s+", " ");
        }

        public async Task<PagedResultDto<AmenityDto>> GetPagingWithFilterAsync(AmenityFilterDto input)
        {
            var searchTerm = !string.IsNullOrEmpty(input.Text) ? $" {NormalizeString.ConvertNormalizeString(input.Text)} " : string.Empty;
            var query = _unitOfWork.GenericRepository<Amenity>()
                                                        .GetQueryable()
                                                        .OrderByDescending(x => x.CreationTime)
                                                        .WhereIf(!string.IsNullOrEmpty(input.Text),
                                                        x => (" " + x.NormalizeFullInfo + " ").Contains(searchTerm))
                                                        .WhereIf(input.Type != null, x => x.Type == input.Type)
                                                        .WhereIf(input.StartDate != null, x => x.CreationTime.AddHours(7).Date >= input.StartDate!.Value.Date)
                                                        .WhereIf(input.EndDate != null, x => x.CreationTime.AddHours(7).Date <= input.EndDate!.Value.Date)
                                                        .Select(x => new AmenityDto()
                                                        {
                                                            Id = x.Id,
                                                            Name = x.Name,
                                                            Type = x.Type.GetDescription(),
                                                            CreationTime = x.CreationTime,
                                                        });

            var data = await query.GetPaged(input.CurrentPage, input.PageSize);
            return ObjectMapper.Map<PagedResult<AmenityDto>, PagedResultDto<AmenityDto>>(data);
        }

        public async Task DeleteAsync(Guid id)
        {
            var amenity = await _unitOfWork.GenericRepository<Amenity>().GetAsync(x => x.Id == id);
            if (amenity == null)
            {
                throw new GlobalException(code: AmenityErrorCode.NotFound,
                        message: L[AmenityErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            _unitOfWork.GenericRepository<Amenity>().Remove(amenity);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<List<AmenityDto>> GetAllAsync()
        {
            var amenities = await _unitOfWork.GenericRepository<Amenity>()
                .GetQueryable()
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new AmenityDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = x.Type.GetDescription(),
                    CreationTime = x.CreationTime,
                })
                .ToListAsync();
            return amenities;
        }

        public async Task<List<RoomAmenityDto>> GetRoomAmenitiesByAmenityAsync(Guid amenityId)
        {
            var result = await _unitOfWork.GenericRepository<RoomAmenity>()
                .GetQueryable()
                .Include(x => x.Room).ThenInclude(y => y!.Branch)
                .Include(x => x.Amenity)
                .Where(x => x.AmenityId == amenityId && !x.Room!.DeletedAt.HasValue)
                .Select(x => new RoomAmenityDto
                {
                    RoomId = x.Room!.Id,
                    RoomNumber = x.Room.RoomNumber,
                    BranchId = x.Room.Branch!.Id,
                    BranchName = x.Room.Branch.Name,
                    AmenityId = x.AmenityId,
                    AmenityName = x.Amenity!.Name,
                    Type = x.Amenity.Type.GetDescription(),
                    Quantity = x.Quantity
                })
                .ToListAsync();

            return result;
        }

        public async Task<List<AmenityDto>> GetByTypeAsync(AmenityTypes? type)
        {
            var result = await _unitOfWork.GenericRepository<Amenity>()
                .GetQueryable()
                .OrderByDescending(x => x.CreationTime)
                .ToListAsync();
            if (type.HasValue)
            {
                result.Where(a => a.Type == type);
            }
            return ObjectMapper.Map<List<Amenity>, List<AmenityDto>>(result); ;
        }

    }
}
