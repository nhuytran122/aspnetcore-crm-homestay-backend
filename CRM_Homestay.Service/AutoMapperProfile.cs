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
    }
}