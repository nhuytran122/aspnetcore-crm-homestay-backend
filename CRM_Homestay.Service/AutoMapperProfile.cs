using AutoMapper;
using CRM_Homestay.Contract.Claims;
using CRM_Homestay.Contract.Roles;
using CRM_Homestay.Contract.SystemSettings;
using CRM_Homestay.Contract.Users;
using CRM_Homestay.Core.Models;
using CRM_Homestay.Entity.Districts;
using CRM_Homestay.Entity.Roles;
using CRM_Homestay.Entity.SystemSettings;
using CRM_Homestay.Entity.Users;
using CRM_Homestay.Entity.Provinces;
using CRM_Homestay.Contract.Locations;
using Ward = CRM_Homestay.Entity.Wards.Ward;
using System.Security.Claims;
using CRM_Homestay.Entity.Branches;
using CRM_Homestay.Contract.Branches;
using CRM_Homestay.Entity.RoomTypes;
using CRM_Homestay.Contract.RoomTypes;
using CRM_Homestay.Entity.RoomPricings;
using CRM_Homestay.Contract.RoomPricings;
using CRM_Homestay.Entity.Amenities;
using CRM_Homestay.Contract.Amenities;
using CRM_Homestay.Core.Extensions;
using CRM_Homestay.Entity.Rooms;
using CRM_Homestay.Contract.Rooms;
using CRM_Homestay.Entity.RoomAmenities;
using CRM_Homestay.Contract.RoomAmenities;
using CRM_Homestay.Contract.HomestayServices;
using CRM_Homestay.Entity.HomestayServices;
using CRM_Homestay.Entity.ServiceItems;
using CRM_Homestay.Contract.ServiceItems;

namespace CRM_Homestay.Service;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {

        CreateMap<UserWithNavigationProperties, UserWithNavigationPropertiesDto>().ReverseMap();
        CreateMap<CreateUserDto, User>().ForMember(dest => dest.NormalizedEmail,
            opt => opt.MapFrom(src => src.Email!.ToUpper()))
            .ForMember(dest => dest.NormalizedUserName,
                opt => opt.MapFrom(src => src.UserName!.ToUpper()));
        CreateMap<UpdateUserDto, User>().ForMember(dest => dest.NormalizedEmail,
            opt => opt.MapFrom(src => src.Email!.ToUpper()));
        CreateMap<UpdateProfileRequestDto, User>().ForMember(dest => dest.NormalizedEmail,
            opt => opt.MapFrom(src => src.Email!.ToUpper()));

        CreateMap<CreateUpdateAddressDto, Address>();

        CreateMap<User, UserDto>().ForMember(dest => dest.FullName,
            opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
        CreateMap<BasicUser, BasicUserDto>().ForMember(dest => dest.FullName,
            opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
        CreateMap<Claim, ClaimDto>();
        CreateMap<CreateUpdateRoleDto, Role>();
        CreateMap<Role, RoleDto>();

        CreateMap<Province, ProvinceDto>();
        CreateMap<District, DistrictDto>();
        CreateMap<Ward, WardDto>();

        CreateMap<SystemSetting, SystemSettingDto>();
        CreateMap<CreateUpdateSystemSettingDto, SystemSetting>();
        CreateMap<Branch, BranchDto>();
        CreateMap<CreateUpdateBranchDto, Branch>();

        // Users
        CreateMap<User, UserPropertiesDto>();
        CreateMap<RoomType, RoomTypeDto>();
        CreateMap<CreateUpdateRoomTypeDto, RoomType>();

        CreateMap<RoomPricing, RoomPricingDto>()
            .ForMember(dest => dest.RoomTypeName,
                opt => opt.MapFrom(src => src.RoomType != null ? src.RoomType.Name : string.Empty));
        CreateMap<CreateRoomPricingDto, RoomPricing>();
        CreateMap<UpdateRoomPricingDto, RoomPricing>();

        CreateMap<Amenity, AmenityDto>()
        .ForMember(dest => dest.Type,
            opt => opt.MapFrom(src => src.Type.GetDescription()));
        CreateMap<CreateUpdateAmenityDto, Amenity>();

        CreateMap<Room, RoomDto>()
            .ForMember(dest => dest.BranchName,
                opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name : string.Empty))
            .ForMember(dest => dest.RoomTypeName,
                opt => opt.MapFrom(src => src.RoomType != null ? src.RoomType.Name : string.Empty));

        CreateMap<Room, RoomDetailDto>();
        CreateMap<CreateRoomDto, Room>();
        CreateMap<UpdateRoomDto, Room>()
            .ForMember(dest => dest.RoomAmenities, opt => opt.Ignore())
            .ForMember(dest => dest.Medias, opt => opt.Ignore());

        CreateMap<RoomAmenity, RoomAmenityDto>()
            .ForMember(dest => dest.RoomNumber,
                opt => opt.MapFrom(src => src.Room != null ? src.Room.RoomNumber : 0))
            .ForMember(dest => dest.AmenityName,
                opt => opt.MapFrom(src => src.Amenity != null ? src.Amenity.Name : null))
            .ForMember(dest => dest.BranchId,
                opt => opt.MapFrom(src => src.Room != null && src.Room.Branch != null ? (Guid?)src.Room.Branch.Id : null))
            .ForMember(dest => dest.BranchName,
                opt => opt.MapFrom(src => src.Room!.Branch != null ? src.Room.Branch.Name : null))
            .ForMember(dest => dest.Type,
                opt => opt.MapFrom(src => src.Amenity != null ? src.Amenity.Type.ToString() : null));

        CreateMap<CreateRoomAmenityDto, RoomAmenity>();
        CreateMap<UpdateRoomAmenityDto, RoomAmenity>();

        CreateMap<CreateAmenityForRoomDto, RoomAmenity>();
        CreateMap<UpdateAmenityForRoomDto, RoomAmenity>();

        CreateMap<CreateUpdateHomestayServiceDto, HomestayService>()
            .ForMember(dest => dest.ServiceItems, opt => opt.Ignore());
        CreateMap<HomestayService, HomestayServiceDto>();
        CreateMap<HomestayService, HomestayServiceDetailDto>();

        CreateMap<CreateServiceItemDto, ServiceItem>();
        CreateMap<UpdateServiceItemDto, ServiceItem>()
            .ForMember(dest => dest.HomestayServiceId, opt => opt.Ignore());
        CreateMap<ServiceItem, ServiceItemDto>()
            .ForMember(dest => dest.ServiceName,
                opt => opt.MapFrom(src => src.HomestayService != null ? src.HomestayService.Name : null));
    }
}