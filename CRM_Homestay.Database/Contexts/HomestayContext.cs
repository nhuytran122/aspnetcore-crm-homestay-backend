using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Consts.Permissions;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;
using CRM_Homestay.Database.Configurations;
using CRM_Homestay.Entity.Amenities;
using CRM_Homestay.Entity.AuditLogs;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.BookingPaymentDetails;
using CRM_Homestay.Entity.BookingPayments;
using CRM_Homestay.Entity.BookingRooms;
using CRM_Homestay.Entity.Bookings;
using CRM_Homestay.Entity.BookingServiceItems;
using CRM_Homestay.Entity.BookingServices;
using CRM_Homestay.Entity.Branches;
using CRM_Homestay.Entity.BranchInventories;
using CRM_Homestay.Entity.Coupons;
using CRM_Homestay.Entity.CustomerGroups;
using CRM_Homestay.Entity.Customers;
using CRM_Homestay.Entity.Districts;
using CRM_Homestay.Entity.ExportProductDetailHistories;
using CRM_Homestay.Entity.FAQs;
using CRM_Homestay.Entity.HomestayMaintenances;
using CRM_Homestay.Entity.HomestayServices;
using CRM_Homestay.Entity.ImportProductDetails;
using CRM_Homestay.Entity.ImportProducts;
using CRM_Homestay.Entity.Media;
using CRM_Homestay.Entity.Medias;
using CRM_Homestay.Entity.Otps;
using CRM_Homestay.Entity.ProductCategories;
using CRM_Homestay.Entity.Products;
using CRM_Homestay.Entity.Provinces;
using CRM_Homestay.Entity.Reviews;
using CRM_Homestay.Entity.RoleClaims;
using CRM_Homestay.Entity.RoomAmenities;
using CRM_Homestay.Entity.RoomPricings;
using CRM_Homestay.Entity.Rooms;
using CRM_Homestay.Entity.RoomTypes;
using CRM_Homestay.Entity.RoomUsages;
using CRM_Homestay.Entity.Rules;
using CRM_Homestay.Entity.ServiceItems;
using CRM_Homestay.Entity.SystemSettings;
using CRM_Homestay.Entity.UserClaims;
using CRM_Homestay.Entity.UserLogins;
using CRM_Homestay.Entity.UserRoles;
using CRM_Homestay.Entity.UserTokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Reflection.Emit;
using System.Security.Claims;
using Role = CRM_Homestay.Entity.Roles.Role;
using User = CRM_Homestay.Entity.Users.User;
using Ward = CRM_Homestay.Entity.Wards.Ward;

namespace CRM_Homestay.Database.Contexts;

public class HomestayContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    //public DbSet<User> Users { get; set; }
    //public DbSet<UserRole> UserRoles { get; set; }
    //public DbSet<Role> Roles { get; set; }
    //public DbSet<RoleClaim> RoleClaims { get; set; }

    public DbSet<Province> Provinces { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Ward> Wards { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<CustomerAccountToken> CustomerAccountTokens { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingPayment> BookingPayments { get; set; }
    public DbSet<BookingPaymentDetail> BookingPaymentDetails { get; set; }
    public DbSet<BookingRoom> BookingRooms { get; set; }
    public DbSet<BookingServiceItem> BookingServiceItems { get; set; }
    public DbSet<BookingService> BookingServices { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<BranchInventory> BranchInventories { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<CustomerGroup> CustomerGroups { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<ExportProductDetailHistory> ExportProductDetailHistories { get; set; }
    public DbSet<FAQ> FAQs { get; set; }
    public DbSet<HomestayMaintenance> HomestayMaintenances { get; set; }
    public DbSet<MaintenanceRoom> MaintenanceRooms { get; set; }
    public DbSet<HomestayService> HomestayServices { get; set; }
    public DbSet<ImportProduct> ImportProducts { get; set; }
    public DbSet<ImportProductDetail> ImportProductDetails { get; set; }
    public DbSet<BaseMedia> BaseMedias { get; set; }
    public DbSet<MediaReview> MediaReviews { get; set; }
    public DbSet<MediaRoom> MediaRooms { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<RoomAmenity> RoomAmenities { get; set; }
    public DbSet<RoomPricing> RoomPricings { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomType> RoomTypes { get; set; }
    public DbSet<RoomUsage> RoomUsages { get; set; }
    public DbSet<Rule> Rules { get; set; }
    public DbSet<ServiceItem> ServiceItems { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }

    public DbSet<OtpCode> OtpCodes { get; set; }
    public DbSet<OtpProviderLog> OtpProviderLogs { get; set; }
    public DbSet<OtpRequest> OtpRequests { get; set; }

    public IHttpContextAccessor _httpContextAccessor { get; set; }
    public Guid CurrentUserId { get; set; }
    private string RootPath { get; set; }

    public HomestayContext(DbContextOptions<HomestayContext> options, IHttpContextAccessor httpContextAccessor, IHostEnvironment environment) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        _httpContextAccessor = httpContextAccessor;
        RootPath = Path.Combine(environment.ContentRootPath, "wwwroot");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //get rid of prefix Asp
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName != null && tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName.Substring(6));
            }
        }

        // builder.ApplyConfiguration(new CategoryAttributeConfiguration());
        // builder.ApplyConfiguration(new AttributeValueConfiguration());

        builder.ApplyConfiguration(new BookingServiceConfiguration());
        SetRelationShip(builder);
        SeedData(builder);
        SetConstraint(builder);
        FilterGlobal(builder);
    }


    public void SetRelationShip(ModelBuilder builder)
    {

        builder.Entity<User>().HasMany(u => u.UserRoles).WithOne(x => x.User)
            .HasForeignKey(r => r.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Role>().HasMany(u => u.UserRoles).WithOne(x => x.Role)
            .HasForeignKey(r => r.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

        builder.Entity<District>()
            .HasOne(x => x.Province)
            .WithMany(x => x.Districts)
            .HasForeignKey(x => x.ProvinceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Ward>()
            .HasOne(x => x.District)
            .WithMany(x => x.Wards)
            .HasForeignKey(x => x.DistrictId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void SetConstraint(ModelBuilder builder)
    {

    }

    public void FilterGlobal(ModelBuilder builder)
    {
        //builder.Entity<User>().HasQueryFilter(x => !x.IsDelete);
        //builder.Entity<Customer>().HasQueryFilter(x => !x.IsDelete);
        //builder.Entity<SpendSlip>().HasQueryFilter(x => !x.IsDeleted);
    }
    public override int SaveChanges()
    {
        CurrentUserId = GetCurrentUserId();

        if (CurrentUserId != Guid.Empty)
        {
            foreach (var entry in ChangeTracker.Entries<IBaseEntity>().ToList())
            {
                ApplyEntityAction(entry.Entity, entry.State);
            }
        }
        var result = base.SaveChanges();
        return result;
    }

    private string GetActionForEntityState(EntityState state)
    {
        switch (state)
        {
            case EntityState.Added:
                return EntityStateEnum.INSERT.ToString();
            case EntityState.Deleted:
                return EntityStateEnum.DELETE.ToString();
            case EntityState.Modified:
                return EntityStateEnum.UPDATE.ToString();
            default:
                return string.Empty;
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        CurrentUserId = GetCurrentUserId();
        if (CurrentUserId != Guid.Empty)
        {
            foreach (var entry in ChangeTracker.Entries<IBaseEntity>().ToList())
            {
                ApplyEntityAction(entry.Entity, entry.State);
            }
        }
        var auditEntries = ChangeTracker.Entries()
                                        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                                        .Where(e => e.Entity is IAuditable)
                                        .ToList();

        if (auditEntries.Any() && CurrentUserId != Guid.Empty)
        {
            var fieldsToExclude = new List<string> { "SubPrice", "LastModificationTime", "LastModifierId" };
            var auditRecordId = Guid.NewGuid();
            foreach (var entry in auditEntries)
            {
                var auditEntry = new AuditLog
                {
                    UserId = CurrentUserId,
                    TableName = entry.Metadata.GetTableName(),
                    Action = GetActionForEntityState(entry.State)
                };

                var oldValues = new Dictionary<string, object?>();
                var newValues = new Dictionary<string, object?>();
                var returnProductObject = new Dictionary<string, object>();

                var primaryKeyPropertyName = "Id";
                var primaryKeyProperty = entry.Entity.GetType().GetProperty(primaryKeyPropertyName);

                if (primaryKeyProperty != null && primaryKeyProperty.GetValue(entry.Entity) != null)
                {
                    auditEntry.ItemId = new Guid(primaryKeyProperty.GetValue(entry.Entity)!.ToString()!);
                }

                foreach (var property in entry.OriginalValues.Properties)
                {
                    var propertyName = property.Name;

                    if (!fieldsToExclude.Contains(propertyName))
                    {
                        var originalValue = entry.OriginalValues[property];
                        var currentValue = entry.CurrentValues[property];
                        if (auditEntry.Action == EntityStateEnum.INSERT.ToString() || auditEntry.Action == EntityStateEnum.DELETE.ToString())
                        {
                            newValues[propertyName] = currentValue;
                        }
                        else
                        {
                            if (!object.Equals(originalValue, currentValue))
                            {
                                oldValues[propertyName] = originalValue;
                                newValues[propertyName] = currentValue;
                            }
                        }
                    }
                }

                auditEntry.OldValues = JsonConvert.SerializeObject(oldValues);
                auditEntry.NewValues = JsonConvert.SerializeObject(newValues);
                auditEntry.AuditRecordId = auditRecordId;

                if (newValues.Count() == 0)
                {
                    continue;
                }

                AuditLogs.Add(auditEntry);
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }
    public Guid GetCurrentUserId()
    {
        var stringId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var tokenType = _httpContextAccessor.HttpContext?.User.FindFirstValue("Type");
        Guid id;
        if (tokenType == null && Guid.TryParse(stringId, out id))
        {
            return id;
        }
        return Guid.Empty;
    }
    public void ApplyEntityAction(IBaseEntity entity, EntityState action)
    {
        switch (action)
        {
            case EntityState.Added:
                {
                    entity.CreationTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                    entity.CreatorId = entity.CreatorId ?? CurrentUserId;
                    break;
                }
            case EntityState.Modified:
                {
                    entity.LastModificationTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                    entity.LastModifierId = CurrentUserId;
                    break;
                }
        }

    }

    public void SeedData(ModelBuilder builder)
    {
        builder.Entity<Role>()
            .HasData(
                new Role
                {
                    Id = Guid.Parse("f6292948-70a1-4804-bb30-f6ea37c44236"),
                    Name = "admin",
                    NormalizedName = "admin".ToUpper(),
                    Description = "Quản lý"
                },
                new Role
                {
                    Id = Guid.Parse("6cc6e2f1-96a1-4091-b4b5-d3b5d52ca5f5"),
                    Name = "receptionist",
                    NormalizedName = "receptionist".ToUpper(),
                    Description = "Nhân viên lễ tân"
                },
                new Role
                {
                    Id = Guid.Parse("2305a873-8b69-480a-bda9-bf5ff1f33b7e"),
                    Name = "technical_staff",
                    NormalizedName = "technical_staff".ToUpper(),
                    Description = "Nhân viên kỹ thuật"
                },
                new Role
                {
                    Id = Guid.Parse("770f2ad4-8203-4947-bdec-f452e0ca280b"),
                    Name = "hr_staff",
                    NormalizedName = RoleEnum.hr_staff.ToString().ToUpper(),
                    Description = "Nhân viên nhân sự"
                },
                new Role
                {
                    Id = Guid.Parse("986e0e56-8aae-4e24-b1fa-a7bb064f5ce4"),
                    Name = "housekeeping_staff",
                    NormalizedName = RoleEnum.housekeeping_staff.ToString().ToUpper(),
                    Description = "Nhân viên dọn dẹp"
                },
                new Role
                {
                    Id = Guid.Parse("34241afb-64bd-4584-b848-99ab710714e7"),
                    Name = "service_staff",
                    NormalizedName = RoleEnum.service_staff.ToString().ToUpper(),
                    Description = "Nhân viên dịch vụ"
                }
            );


        //a hasher to hash the password before seeding the user to the db
        var hasher = new PasswordHasher<User>();

        //Seeding the User to AspNetUsers table
        builder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("b480e58f-14a5-414c-b54b-89d168f833b2"),
                FirstName = "Admin",
                LastName = "Nguyen",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                IsActive = true,
                PasswordHash = hasher.HashPassword(new User(), "a123456"),
                SecurityStamp = Guid.NewGuid().ToString(),
                NormalizeFullInfo = "ADMIN NGUYEN"
            }
        );


        //Seeding the relation between our user and role to AspNetUserRoles table
        builder.Entity<UserRole>().HasData(
            new UserRole
            {
                RoleId = Guid.Parse("f6292948-70a1-4804-bb30-f6ea37c44236"),
                UserId = Guid.Parse("b480e58f-14a5-414c-b54b-89d168f833b2")
            }
        );
        var roleClaims = new List<RoleClaim>();
        int index = 1;
        foreach (Type nestedType in typeof(AccessClaims).GetNestedTypes())
        {

            foreach (var field in nestedType.GetFields())
            {
                roleClaims.Add(new RoleClaim()
                {
                    Id = index,
                    ClaimType = ExtendClaimTypes.Permission,
                    ClaimValue = field.GetValue(null)?.ToString(),
                    RoleId = Guid.Parse("f6292948-70a1-4804-bb30-f6ea37c44236")
                });
                index++;

            }
        }
        builder.Entity<RoleClaim>().HasData(roleClaims);

        //HttpClient client = new HttpClient();
        //var result = client
        //    .GetAPIAsync<List<ProvinceWithDetail>>("https://provinces.open-api.vn/api/?depth=3").Result;
        var path = Path.Combine(RootPath, "Data", "a27a0dc6-43e2-4f1a-vietnam-provinces-9209-2eb193d2fd7e.json");
        if (File.Exists(path))
        {
            var dataJson = File.ReadAllText(path);
            var result = JsonConvert.DeserializeObject<List<ProvinceWithDetail>>(dataJson);

            var provinces = new List<Province>();
            var districts = new List<District>();
            var wards = new List<Ward>();

            foreach (var province in result!)
            {
                provinces.Add(new Province()
                {
                    Id = province.Code,
                    Code = province.Code,
                    Name = province.Name,
                    DivisionType = province.Division_Type
                });

                foreach (var district in province.Districts!)
                {
                    districts.Add(new District()
                    {
                        Id = district.Code,
                        Code = district.Code,
                        Name = district.Name,
                        DivisionType = district.Division_Type,
                        ProvinceId = province.Code
                    });

                    foreach (var ward in district.Wards!)
                    {
                        wards.Add(new Ward()
                        {
                            DivisionType = ward.Division_Type,
                            Code = ward.Code,
                            Id = ward.Code,
                            Name = ward.Name,
                            DistrictId = district.Code
                        });
                    }
                }
            }
            builder.Entity<Province>().HasData(provinces);
            builder.Entity<District>().HasData(districts);
            builder.Entity<Ward>().HasData(wards);

            builder.Entity<SystemSetting>()
            .HasData(
                new SystemSetting
                {
                    Id = Guid.Parse("d10dac54-b88e-4105-940b-34ad1b2bf4fd"),
                    SystemName = ConfigKey.RoomUsage,
                    ConfigKey = ConfigKey.CleaningMinutes,
                    ConfigValue = "60",
                    Description = "Thời gian (phút) dọn dẹp phòng"
                },
                new SystemSetting
                {
                    Id = Guid.Parse("a34726e0-fe3f-442e-a337-48ae997becf0"),
                    SystemName = ConfigKey.RoomPricing,
                    ConfigKey = ConfigKey.OvernightStartTime,
                    ConfigValue = "22:00:00",
                    Description = "Thời gian bắt đầu tính giá qua đêm (giờ check-in được tính là giá qua đêm nếu >= thời gian này)"
                },
                new SystemSetting
                {
                    Id = Guid.Parse("b21c690b-4042-4407-9d78-e693fbf7ae46"),
                    SystemName = ConfigKey.RoomPricing,
                    ConfigKey = ConfigKey.OvernightEndTime,
                    ConfigValue = "08:00:00",
                    Description = "Thời gian kết thúc tính giá qua đêm (giờ check-out được tính là giá qua đêm nếu <= thời gian này)"
                },
                new SystemSetting
                {
                    Id = Guid.Parse("1977ace4-af1b-4344-8ec8-8e7c43a3ca2b"),
                    SystemName = ConfigKey.IncentiveCoupon,
                    ConfigKey = ConfigKey.DiscountType,
                    ConfigValue = "2"
                },
                new SystemSetting
                {
                    Id = Guid.Parse("a5511daa-0ef1-40c7-82bc-ca05ac3dc89a"),
                    SystemName = ConfigKey.IncentiveCoupon,
                    ConfigKey = ConfigKey.DiscountValue,
                    ConfigValue = "10000"
                }
            );

            // Seed config Zalo_OA
            builder.Entity<SystemSetting>().HasData(
                new SystemSetting
                {
                    Id = Guid.Parse("f6c8d07a-5a64-4f17-b36c-7a42b9c8a001"),
                    SystemName = ConfigKey.ZaloOAConfigKey,
                    ConfigKey = nameof(ZaloSystemConfigKeys.DEVELOPMENT_MODE),
                    ConfigValue = "development"
                },
                new SystemSetting
                {
                    Id = Guid.Parse("2a1d4c5e-21f0-4a3e-9c92-58f5b8fbb002"),
                    SystemName = ConfigKey.ZaloOAConfigKey,
                    ConfigKey = nameof(ZaloSystemConfigKeys.TEMPLATE_ID),
                    ConfigValue = "956856"
                },
                new SystemSetting
                {
                    Id = Guid.Parse("b39c2ff4-41e8-4663-96e1-bf9d6b3fa003"),
                    SystemName = ConfigKey.ZaloOAConfigKey,
                    ConfigKey = nameof(ZaloSystemConfigKeys.ADMIN_PHONE),
                    ConfigValue = "0987456321"
                },
                new SystemSetting
                {
                    Id = Guid.Parse("c6e1d902-7dcb-4f37-8c14-944feebca004"),
                    SystemName = ConfigKey.ZaloOAConfigKey,
                    ConfigKey = nameof(ZaloSystemConfigKeys.APP_ID),
                    ConfigValue = "9652566698963****"
                },
                new SystemSetting
                {
                    Id = Guid.Parse("4b7e5f5a-55fd-46ab-8576-739b7c1da005"),
                    SystemName = ConfigKey.ZaloOAConfigKey,
                    ConfigKey = nameof(ZaloSystemConfigKeys.SECRET_KEY),
                    ConfigValue = "jhtitiytmjhjhkjkh****"
                },
                new SystemSetting
                {
                    Id = Guid.Parse("d19f45f8-0f47-4d65-8f8d-0b64e1dda006"),
                    SystemName = ConfigKey.ZaloOAConfigKey,
                    ConfigKey = nameof(ZaloSystemConfigKeys.REFRESH_TOKEN),
                    ConfigValue = "wxKYU4QW_aRgj2LwGljktytyutyu********"
                },
                new SystemSetting
                {
                    Id = Guid.Parse("9a5c8a90-0a04-45a3-bef0-cc66d8b1a007"),
                    SystemName = ConfigKey.ZaloOAConfigKey,
                    ConfigKey = nameof(ZaloSystemConfigKeys.ACCESS_TOKEN),
                    ConfigValue = "O_w29KxEtbnY_jklkl6Nv******"
                },
                new SystemSetting
                {
                    Id = Guid.Parse("03a1f3c3-baf7-41f1-bc4b-36f7a6e8a008"),
                    SystemName = ConfigKey.ZaloOAConfigKey,
                    ConfigKey = nameof(ZaloSystemConfigKeys.EXPIRES_IN),
                    ConfigValue = "2025-07-31T10:41:12.8294958Z"
                },
                new SystemSetting
                {
                    Id = Guid.Parse("8c17c3f1-d59e-45f9-9f63-81d85c8da009"),
                    SystemName = ConfigKey.ZaloOAConfigKey,
                    ConfigKey = nameof(ZaloSystemConfigKeys.COUNT_TO_LOCK),
                    ConfigValue = "4"
                }
            );

        }

    }
}