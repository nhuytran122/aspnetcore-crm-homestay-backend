using AspNetCoreRateLimit;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using System.Reflection;
using System.Text;
using WatchDog;
using WatchDog.src.Enums;
using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.AuthorizationHandlers;
using CRM_Homestay.Core.Consts.Permissions;
using CRM_Homestay.Core.Validations;
using CRM_Homestay.Database.Contexts;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Localization;
using CRM_Homestay.Service;
using CRM_Homestay.Contract.Users;
using CRM_Homestay.Contract.Roles;

using CRM_Homestay.Entity.Users;
using CRM_Homestay.Entity.Roles;
using CRM_Homestay.Service.Users;
using CRM_Homestay.Contract.Locations;
using CRM_Homestay.Service.Locations;
using CRM_Homestay.Service.Password;
using CRM_Homestay.Contract.Password;
using CRM_Homestay.Service.Roles;
using CRM_Homestay.Contract.Uploads;
using CRM_Homestay.Service.Uploads;
using CRM_Homestay.Contract.Branches;
using CRM_Homestay.Service.Branches;
using CRM_Homestay.Contract.RoomTypes;
using CRM_Homestay.Service.RoomTypes;
using CRM_Homestay.Contract.RoomPricings;
using CRM_Homestay.Service.RoomPricings;
using CRM_Homestay.Service.Amenities;
using CRM_Homestay.Contract.Amenities;

namespace CRM_Homestay.App.ServiceInstallers;

/// <summary>
/// Installers
/// </summary>
public static class Installers
{
    private static IServiceCollection _services { get; set; } = new ServiceCollection();
    private static IConfiguration? _configuration { get; set; }

    /// <summary>
    /// InstallServices
    /// </summary>
    /// <param name="service"></param>
    /// <param name="configuration"></param>
    public static void InstallServices(this IServiceCollection service, IConfiguration configuration)
    {
        _services = service;
        _configuration = configuration;
        InstallSwagger();
        InstallAutoMapper();
        RegisterService();
        InstallAuthorizationHandler();
        InstallCors();
        InstallHealCheck();
        InstallRateLimit();
        InstallRedis();
        InstallPostgresql();
        InstallFluentValidation();
        InstallLocalization();
        InstallJwt();
        InstallWatchLog();
    }

    private static void InstallSwagger()
    {
        _services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Homestay API", Version = "v1" });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
    }

    private static void InstallAutoMapper()
    {
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new AutoMapperProfile());
            mc.CreateMap(typeof(Entity.Bases.PagedResult<>), typeof(PagedResultDto<>));
        });
        IMapper mapper = mapperConfig.CreateMapper();
        _services.AddSingleton(mapper);
    }

    private static void InstallHealCheck()
    {
        _services.AddHealthChecks()
    .AddNpgSql(_configuration!.GetConnectionString("CRM_Homestay")!);

    }

    private static void InstallRateLimit()
    {
        _services.AddMemoryCache();
        _services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        _services.Configure<IpRateLimitOptions>(_configuration!.GetSection("IpRateLimitingSettings"));
        _services.AddInMemoryRateLimiting();
    }

    private static void InstallRedis()
    {
        _services.AddStackExchangeRedisCache(options =>
        {
            //server redis
            options.Configuration = _configuration!.GetConnectionString("Redis");
        });
    }

    private static void InstallPostgresql()
    {
        var dataSource = new NpgsqlDataSourceBuilder(_configuration!.GetConnectionString("CRM_Homestay"))
            .EnableDynamicJson()
            .Build();

        _services.AddDbContext<HomestayContext>(options =>
        {
            options.UseNpgsql(
                dataSource,
                b => b.MigrationsAssembly("CRM_Homestay.WebApi").UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            );
        }, ServiceLifetime.Transient, ServiceLifetime.Transient);

        _services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<HomestayContext>()
            .AddDefaultTokenProviders();

        _services.Configure<IdentityOptions>(options =>
        {
            // set up Password
            options.Password.RequireDigit = false; // Không bắt phải có số
            options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
            options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
            options.Password.RequireUppercase = false; // Không bắt buộc chữ in
            options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
            options.Password.RequiredUniqueChars = 0; // Số ký tự riêng biệt

            // Cấu hình Lockout - khóa user
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
            options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
            options.Lockout.AllowedForNewUsers = true;
            // Cấu hình về User.
            options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = false; // Email là duy nhất
            // configure login
            options.SignIn.RequireConfirmedEmail = false; // Cấu hình xác thực địa chỉ email (email phải tồn tại)
            options.SignIn.RequireConfirmedPhoneNumber = false;
            // Xác thực số điện thoại
        });
    }

    /// <summary>
    /// InstallCors
    /// </summary>
    public static void InstallCors()
    {
        _services.AddCors(options => options.AddPolicy("ApiCorsPolicy", opt =>
        {
            //1 địa chỉ bất kì
            opt.WithOrigins(_configuration!.GetSection("Cors").Get<string[]>()!)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }));
    }

    /// <summary>
    /// InstallWatchLog
    /// </summary>
    public static void InstallWatchLog()
    {
        _services.AddWatchDogServices(opt =>
        {
            opt.IsAutoClear = true;
            opt.ClearTimeSchedule = WatchDogAutoClearScheduleEnum.Weekly;
            opt.DbDriverOption = WatchDogDbDriverEnum.PostgreSql;
            opt.SetExternalDbConnString = _configuration!.GetConnectionString("CRM_Homestay");
        });
    }


    private static void InstallFluentValidation()
    {
        _services.AddFluentValidationAutoValidation();
        _services.AddFluentValidationClientsideAdapters();
        _services.AddValidatorsFromAssemblyContaining<IValidatorService>();
    }

    private static void InstallLocalization()
    {
        _services.AddSingleton<Localizer>();
    }

    private static void InstallJwt()
    {
        _services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = _configuration!["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                LifetimeValidator = LifetimeValidator,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };
            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/message-hub") || path.StartsWithSegments("/user-notification-hub")))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                },
            };
        });
    }

    /// <summary>
    /// LifetimeValidator
    /// </summary>
    /// <param name="notBefore"></param>
    /// <param name="expires"></param>
    /// <param name="tokenToValidate"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate, TokenValidationParameters @param)
    {
        return expires != null && expires > DateTime.UtcNow;
    }

    private static void InstallAuthorizationHandler()
    {
        _services.AddScoped<IAuthorizationHandler, AppAuthorizationHandler>();

        _services.AddAuthorization(options =>
        {
            foreach (Type nestedType in typeof(AccessClaims).GetNestedTypes())
            {
                foreach (var field in nestedType.GetFields())
                {
                    options.AddPolicy(field.GetValue(null)!.ToString()!,
                        builder => { builder.AddRequirements(new ClaimRequirement(field.GetValue(null)!.ToString()!)); });
                }
            }
        });
    }

    private static void RegisterService()
    {
        _services.AddScoped<ILocalizer, Localizer>();
        _services.AddScoped<IUnitOfWork, UnitOfWork>();
        _services.AddScoped<ILocationService, LocationService>();
        _services.AddScoped<ILocationServiceShared, LocationServiceShared>();
        _services.AddScoped<IUploadService, UploadService>();
        _services.AddScoped<IRoleService, RoleService>();
        _services.AddScoped<IPasswordHasher, PasswordHasher>();
        _services.AddScoped<IUserService, UserService>();
        _services.AddScoped<IBranchService, BranchService>();
        _services.AddScoped<IRoomTypeService, RoomTypeService>();
        _services.AddScoped<IRoomPricingService, RoomPricingService>();
        _services.AddScoped<IAmenityService, AmenityService>();
    }
}