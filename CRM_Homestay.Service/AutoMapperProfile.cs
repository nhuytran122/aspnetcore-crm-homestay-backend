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
using CRM_Homestay.Entity.CustomerGroups;
using CRM_Homestay.Contract.CustomerGroups;
using CRM_Homestay.Entity.Customers;
using CRM_Homestay.Contract.Customers;
using CRM_Homestay.Entity.FAQs;
using CRM_Homestay.Contract.FAQs;
using CRM_Homestay.Contract.Rules;
using CRM_Homestay.Entity.Rules;
using CRM_Homestay.Entity.Coupons;
using CRM_Homestay.Contract.Coupons;
using CRM_Homestay.Entity.RoomUsages;
using CRM_Homestay.Contract.RoomUsages;
using CRM_Homestay.Entity.Bookings;
using CRM_Homestay.Contract.Bookings;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Entity.BookingRooms;
using CRM_Homestay.Entity.BookingServices;
using CRM_Homestay.Contract.BookingServices;
using CRM_Homestay.Entity.BookingServiceItems;
using CRM_Homestay.Contract.OtpCodes;
using CRM_Homestay.Entity.Otps;

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

        CreateMap<CustomerGroup, CustomerGroupDto>();

        CreateMap<CreateUpdateCustomerGroupDto, CustomerGroup>()
            .ForMember(dest => dest.NormalizedName,
                opt => opt.MapFrom(src => src.Name!.ToUpper()))
            .ForMember(dest => dest.NormalizedCode,
                opt => opt.MapFrom(src => src.Code!.ToUpper()));

        CreateMap<Customer, CustomerDto>().ForMember(dest => dest.FullName,
            opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
        CreateMap<CreateUpdateCustomerDto, Customer>()
            .ForMember(dest => dest.NormalizedEmail,
                opt => opt.MapFrom(src => src.Email!.ToUpper()))
            .ForMember(dest => dest.NormalizedCompanyName,
                opt => opt.MapFrom(src => src.CompanyName!.ToUpper()));
        CreateMap<CustomerWithNavigationProperties, CustomerWithNavigationPropertiesDto>();

        CreateMap<FAQ, FAQDto>();
        CreateMap<CreateUpdateFAQDto, FAQ>();

        CreateMap<Rule, RuleDto>();
        CreateMap<CreateUpdateRuleDto, Rule>();

        CreateMap<Coupon, CouponDto>().ReverseMap();
        CreateMap<CreateUpdateCouponDto, Coupon>().ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); ;

        CreateMap<RoomUsage, RoomUsageDto>()
            .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room!.RoomNumber));
        CreateMap<BookingRoom, BookingRoomDto>()
            .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.RoomId))
            .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room!.RoomNumber))
            .ForMember(dest => dest.RoomTypeId, opt => opt.MapFrom(src => src.Room!.RoomType != null ? src.Room.RoomType.Id : Guid.Empty))
            .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.Room!.RoomType != null ? src.Room.RoomType.Name : string.Empty))
            .ForMember(dest => dest.GuestCounts, opt => opt.MapFrom(src => src.GuestCounts));

        CreateMap<Booking, BookingDto>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Id : Guid.Empty))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src =>
                src.Customer != null
                    ? (src.Customer.Type == CustomerTypes.Individual
                        ? $"{src.Customer.FirstName ?? string.Empty} {src.Customer.LastName ?? string.Empty}".Trim()
                        : src.Customer.CompanyName ?? string.Empty)
                    : string.Empty
            ))
            .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src => src.BookingRooms));

        CreateMap<CreateBookingDto, Booking>();
        CreateMap<RoomPricing, BookingPricingSnapshot>();

        CreateMap<BookingService, BookingServiceDto>()
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service!.Name))
            .ForMember(dest => dest.AssignedStaffName, opt => opt.MapFrom(src => src.AssignedStaff != null ? src.AssignedStaff.FullName : null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.BookingServiceItems));

        CreateMap<BookingServiceItem, BookingServiceItemDto>()
            .ForMember(dest => dest.Identifier, opt => opt.MapFrom(src => src.ServiceItem!.Identifier));

        CreateMap<SendOtpCodeDto, OtpCode>();
        CreateMap<OtpProviderLogDto, OtpProviderLog>()
            .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.ProviderName ?? "")) 
            .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => (int?)src.StatusCode));   
    }
}