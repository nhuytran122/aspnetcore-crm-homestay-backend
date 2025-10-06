using AutoMapper;
using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.CustomerAccounts;
using CRM_Homestay.Contract.Customers;
using CRM_Homestay.Contract.Locations;
using CRM_Homestay.Contract.Password;
using CRM_Homestay.Contract.Users;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Core.Models;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Customers;
using CRM_Homestay.Entity.Otps;
using CRM_Homestay.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace CRM_Homestay.Service.CustomerAccounts;

public class CustomerAccountService : BaseService, ICustomerAccountService
{
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocationServiceShared _locationServiceShared;

    private readonly Guid DefaultGroupId = Guid.Parse("f98a3408-046a-4d39-89fd-dd567dbdecfe");
    private static readonly CustomerStatuses[] ExcludedStatuses = new[]
    {
        CustomerStatuses.InActive,
        CustomerStatuses.Deleted,
        CustomerStatuses.Banned
    };

    public CustomerAccountService(IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l, IPasswordHasher passwordHasher,
        IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILocationServiceShared locationServiceShared) : base(unitOfWork, mapper, l)
    {
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _locationServiceShared = locationServiceShared;
    }

    public async Task<SignUpResponseDto> SignUpAsync(SignUpRequestDto request)
    {
        Gender genderEnum;
        if (!Enum.TryParse(request.Gender, true, out genderEnum))
        {
            throw new GlobalException(code: CustomerAccountErrorCode.InvalidGender,
                                      message: L[CustomerAccountErrorCode.InvalidGender],
                                      statusCode: HttpStatusCode.BadRequest);
        }

        var existingCustomer = await _unitOfWork.GenericRepository<CustomerAccount>()
            .GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Status != CustomerStatuses.Deleted && (x.PhoneNumber == request.PhoneNumber || (!string.IsNullOrEmpty(request.Email) && x.Email == request.Email)));

        if (existingCustomer != null)
        {
            if (existingCustomer.PhoneNumber == request.PhoneNumber)
            {
                throw new GlobalException(code: CustomerAccountErrorCode.PhoneNumberAlreadyExist,
                                    message: L[CustomerAccountErrorCode.PhoneNumberAlreadyExist],
                                    statusCode: HttpStatusCode.BadRequest);
            }
            if (existingCustomer.Email == request.Email)
            {
                throw new GlobalException(code: CustomerAccountErrorCode.EmailAlreadyUsed,
                                    message: L[CustomerAccountErrorCode.EmailAlreadyUsed],
                                    statusCode: HttpStatusCode.BadRequest);
            }
        }
        using (_unitOfWork.BeginTransaction())
        {
            try
            {
                var newCustomer = new CustomerAccount
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber!,
                    Email = request.Email,
                    Gender = genderEnum,
                    Status = CustomerStatuses.Pending,
                };
                newCustomer.PasswordHash = _passwordHasher.Hash(newCustomer, request.Password!);

                await _unitOfWork.GenericRepository<CustomerAccount>().AddAsync(newCustomer);

                var payload = new CustomerAccountTokenDto
                {
                    CustomerAccountId = newCustomer.Id,
                    ExpiresIn = DateTime.UtcNow.AddMonths(6),
                    Type = TokenTypes.customer_token.ToString(),
                };
                var accessToken = GenerateCustomerAccountJwtToken(payload);

                await _unitOfWork.GenericRepository<CustomerAccountToken>().AddAsync(new CustomerAccountToken
                {
                    Token = accessToken,
                    CustomerAccountId = newCustomer.Id,
                });

                await _unitOfWork.SaveChangeAsync();
                var result = new SignUpResponseDto
                {
                    AccessToken = accessToken,
                    Id = newCustomer.Id,
                };
                _unitOfWork.Commit();

                return result;
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

    public async Task LogoutAsync(Guid customerAccountId, string token)
    {
        var customerAccountToken = await _unitOfWork.GenericRepository<CustomerAccountToken>().GetQueryable()
            .FirstOrDefaultAsync(x => x.CustomerAccountId == customerAccountId && x.Token == token);
        if (customerAccountToken != null)
        {
            _unitOfWork.GenericRepository<CustomerAccountToken>().Remove(customerAccountToken);
            await _unitOfWork.SaveChangeAsync();
        }
    }

    public async Task<VerifyAccountResponseDto> VerifyAsync()
    {
        var otpCode = _httpContextAccessor.HttpContext.Items["OtpToken"] as OtpCode ?? new OtpCode();

        using (_unitOfWork.BeginTransaction())
        {
            try
            {
                var customerAccount = await _unitOfWork.GenericRepository<CustomerAccount>().GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == Guid.Parse(otpCode.ReferenceId) && x.Status == CustomerStatuses.Pending);
                if (customerAccount == null)
                {
                    throw new GlobalException(code: CustomerAccountErrorCode.AccountNotFound,
                                 message: L[CustomerAccountErrorCode.AccountNotFound],
                                 statusCode: HttpStatusCode.NotFound);
                }

                var currentTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                if (otpCode.RecipientTypes == RecipientTypes.email.ToString())
                {
                    customerAccount.EmailVerifiedAt = currentTime;
                }
                else if (otpCode.RecipientTypes == RecipientTypes.phone.ToString() || otpCode.RecipientTypes == RecipientTypes.dump.ToString())
                {
                    customerAccount.PhoneVerifiedAt = currentTime;
                }

                otpCode.Status = OtpStatuses.Used;
                customerAccount.Status = CustomerStatuses.Active;

                // Kiểm tra xem có tài khoản nào sử dụng số điện thoại này chưa ở trạng thái xóa mềm
                var existingAccount = await _unitOfWork.GenericRepository<CustomerAccount>().GetQueryable()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.PhoneNumber == customerAccount.PhoneNumber && x.Status == CustomerStatuses.Deleted);

                // Nếu có tài khoản đã xóa mềm thì thực hiện hard delete
                if (existingAccount != null)
                {
                    // Thực hiện hard delete
                    // Todo: Uncomment the following lines if you want to hard delete the account
                    // _unitOfWork.GenericRepository<CustomerAccount>().Remove(existingAccount);
                    // await _unitOfWork.SaveChangeAsync();

                    // Tạo customer mới và gán thông tin cho customerAccount
                    var customer = new Customer
                    {
                        PhoneNumber = customerAccount.PhoneNumber,
                        FirstName = customerAccount.FirstName,
                        LastName = customerAccount.LastName,
                        Email = customerAccount.Email ?? "",
                        NormalizedEmail = customerAccount.Email?.ToUpper() ?? "",
                        Gender = customerAccount.Gender,
                        GroupId = DefaultGroupId,
                        NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{customerAccount.FirstName} {customerAccount.LastName}")
                    };
                    await _unitOfWork.GenericRepository<Customer>().AddAsync(customer);
                    await _unitOfWork.SaveChangeAsync();

                    customerAccount.CustomerId = customer.Id;
                    _unitOfWork.GenericRepository<CustomerAccount>().Update(customerAccount);
                    await _unitOfWork.SaveChangeAsync();

                    await GetCustomerPointWalletAsync(customer.Id);

                    _unitOfWork.Commit();

                    return new VerifyAccountResponseDto
                    {
                        Success = true
                    };
                }

                // Lấy thông tin customer dựa vào sdt
                var metaCustomer = await BuildMapAccountMetaAsync(customerAccount);

                // Nếu chưa có customer thì tạo mới với thông tin từ customerAccount
                if (metaCustomer.Item2 == null)
                {
                    var customer = new Customer
                    {
                        PhoneNumber = customerAccount.PhoneNumber,
                        FirstName = customerAccount.FirstName,
                        LastName = customerAccount.LastName,
                        Email = customerAccount.Email ?? "",
                        NormalizedEmail = customerAccount.Email?.ToUpper() ?? "",
                        Gender = customerAccount.Gender,
                        GroupId = DefaultGroupId,
                        NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{customerAccount.FirstName} {customerAccount.LastName}")
                    };

                    await _unitOfWork.GenericRepository<Customer>().AddAsync(customer);
                    await _unitOfWork.SaveChangeAsync();

                    customerAccount.CustomerId = customer.Id;

                    _unitOfWork.GenericRepository<OtpCode>().Update(otpCode);
                    _unitOfWork.GenericRepository<CustomerAccount>().Update(customerAccount);
                    await _unitOfWork.SaveChangeAsync();

                    // Tạo Point Wallet cho customer mới
                    await GetCustomerPointWalletAsync(customer.Id);

                    _unitOfWork.Commit();

                    return new VerifyAccountResponseDto
                    {
                        Success = true
                    };
                }

                _unitOfWork.GenericRepository<OtpCode>().Update(otpCode);
                _unitOfWork.GenericRepository<CustomerAccount>().Update(customerAccount);
                await _unitOfWork.SaveChangeAsync();
                _unitOfWork.Commit();

                return new VerifyAccountResponseDto
                {
                    Success = true,
                    Meta = metaCustomer.Item1
                };
            }
            catch (GlobalException)
            {
                _unitOfWork.Rollback();
                throw;
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

    public async Task<BaseResponse> MapWithCustomerAsync(Guid customerAccountId, MapCustomerRequestDto request)
    {
        using (_unitOfWork.BeginTransaction())
        {
            try
            {
                var customerAccount = await _unitOfWork.GenericRepository<CustomerAccount>().GetQueryable()
                    .FirstOrDefaultAsync(x => x.Id == customerAccountId && x.Status == CustomerStatuses.Active);
                if (customerAccount == null)
                {
                    throw new GlobalException(code: CustomerAccountErrorCode.AccountNotFound,
                        message: L[CustomerAccountErrorCode.AccountNotFound],
                        statusCode: HttpStatusCode.NotFound);
                }

                if (customerAccount.CustomerId != null)
                {
                    throw new GlobalException(code: CustomerAccountErrorCode.CustomerAccountAlreadyLinked,
                            message: L[CustomerAccountErrorCode.CustomerAccountAlreadyLinked],
                            statusCode: HttpStatusCode.BadRequest);
                }

                // Trường hợp người dùng chọn không map tài khoản với customer
                if (request.CustomerId == null || request.CustomerId == Guid.Empty)
                {
                    var customer = new Customer
                    {
                        PhoneNumber = customerAccount.PhoneNumber,
                        FirstName = customerAccount.FirstName,
                        LastName = customerAccount.LastName,
                        Email = customerAccount.Email ?? "",
                        NormalizedEmail = customerAccount.Email!.ToUpper(),
                        Gender = customerAccount.Gender,
                        GroupId = DefaultGroupId
                    };
                    customer.NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{customer.FirstName} {customer.LastName}");

                    await _unitOfWork.GenericRepository<Customer>().AddAsync(customer);
                    await _unitOfWork.SaveChangeAsync();
                    customerAccount.CustomerId = customer.Id;
                }
                else
                {
                    var meta = await BuildMapAccountMetaAsync(customerAccount);
                    if (meta.Item2 == null || meta.Item2.Id != request.CustomerId)
                    {
                        throw new GlobalException(code: CustomerAccountErrorCode.CustomerNotFound,
                           message: L[CustomerAccountErrorCode.CustomerNotFound],
                           statusCode: HttpStatusCode.BadRequest);
                    }

                    // Trường hợp người dùng chọn map tài khoản với customer đã có sẵn
                    var customer = await _unitOfWork.GenericRepository<Customer>().GetQueryable()
                        .FirstOrDefaultAsync(x => x.Id == request.CustomerId && !x.DeletedAt.HasValue);
                    if (customer == null)
                    {
                        throw new GlobalException(code: CustomerAccountErrorCode.CustomerNotFound,
                            message: L[CustomerAccountErrorCode.CustomerNotFound],
                            statusCode: HttpStatusCode.NotFound);
                    }

                    // Check sdt của customerAccount có khớp với customer không
                    if (!string.IsNullOrEmpty(customer.PhoneNumber) && !customer.PhoneNumber.Contains(customerAccount.PhoneNumber))
                    {
                        throw new GlobalException(code: CustomerAccountErrorCode.PhoneNumberMismatch,
                            message: L[CustomerAccountErrorCode.PhoneNumberMismatch],
                            statusCode: HttpStatusCode.BadRequest);
                    }

                    // Check customer đã liên kết với tài khoản khách hàng khác chưa dưới db
                    var existingCustomerAccount = await _unitOfWork.GenericRepository<CustomerAccount>().GetQueryable().AsNoTracking().AnyAsync(x => x.CustomerId == customer.Id);
                    if (existingCustomerAccount)
                    {
                        throw new GlobalException(code: CustomerAccountErrorCode.CustomerAlreadyLinked,
                            message: L[CustomerAccountErrorCode.CustomerAlreadyLinked],
                            statusCode: HttpStatusCode.BadRequest);
                    }

                    // Update thông tin customerAccount với thông tin từ customer
                    // customer.PhoneNumber = customerAccount.PhoneNumber;
                    customer.FirstName = customerAccount.FirstName;
                    customer.LastName = customerAccount.LastName;
                    customer.Gender = customerAccount.Gender;
                    if (!string.IsNullOrEmpty(customerAccount.Email))
                    {
                        customer.Email = customerAccount.Email;
                        customer.NormalizedEmail = customerAccount.Email!.ToUpper();
                    }
                    customer.NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{customer.FirstName} {customer.LastName} {customer.CompanyName}");

                    _unitOfWork.GenericRepository<Customer>().Update(customer);
                    customerAccount.CustomerId = customer.Id;
                }

                _unitOfWork.GenericRepository<CustomerAccount>().Update(customerAccount);
                await _unitOfWork.SaveChangeAsync();

                // Tạo Point Wallet cho customer mới
                await GetCustomerPointWalletAsync(customerAccount.CustomerId.Value);

                _unitOfWork.Commit();
                return new BaseResponse();
            }
            catch (GlobalException)
            {
                _unitOfWork.Rollback();
                throw;
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

    public async Task<SignUpResponseDto> SignInAsync(SignInRequestDto request)
    {
        var customerAccount = await _unitOfWork.GenericRepository<CustomerAccount>().GetQueryable().OrderByDescending(x => x.CreationTime).FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber && x.DeletedAt == null);
        if (customerAccount == null)
        {
            throw new GlobalException(code: CustomerAccountErrorCode.InvalidCredentials,
                                      message: L[CustomerAccountErrorCode.InvalidCredentials],
                                      statusCode: HttpStatusCode.Unauthorized);
        }

        if (!_passwordHasher.Verify(customerAccount, request.Password!, customerAccount.PasswordHash))
        {
            throw new GlobalException(code: CustomerAccountErrorCode.InvalidCredentials,
                                      message: L[CustomerAccountErrorCode.InvalidCredentials],
                                      statusCode: HttpStatusCode.Unauthorized);
        }

        // Kiểm tra trạng thái tài khoản
        switch (customerAccount.Status)
        {
            case CustomerStatuses.Deleted:
                throw new GlobalException(
                    code: CustomerAccountErrorCode.InvalidCredentials,
                    message: L[CustomerAccountErrorCode.InvalidCredentials],
                    statusCode: HttpStatusCode.Unauthorized);

            // case CustomerStatuses.Pending:
            //     throw new GlobalException(
            //         code: CustomerAccountErrorCode.UnverifiedAccount,
            //         message: L[CustomerAccountErrorCode.UnverifiedAccount],
            //         statusCode: HttpStatusCode.Forbidden);

            case CustomerStatuses.InActive:
            case CustomerStatuses.Banned:
                throw new GlobalException(
                    code: CustomerAccountErrorCode.AccountBanned,
                    message: L[CustomerAccountErrorCode.AccountBanned],
                    statusCode: HttpStatusCode.Forbidden);
        }

        var payload = new CustomerAccountTokenDto
        {
            CustomerAccountId = customerAccount.Id,
            ExpiresIn = DateTime.UtcNow.AddMonths(6),
            Type = TokenTypes.customer_token.ToString(),
            CustomerId = customerAccount.CustomerId
        };
        var accessToken = GenerateCustomerAccountJwtToken(payload);

        await _unitOfWork.GenericRepository<CustomerAccountToken>().AddAsync(new CustomerAccountToken
        {
            Token = accessToken,
            CustomerAccountId = customerAccount.Id,
        });
        await _unitOfWork.SaveChangeAsync();

        var result = new SignUpResponseDto
        {
            Id = customerAccount.Id,
            AccessToken = accessToken
        };

        return result;
    }

    public async Task<BaseResponse> ForgotPasswordAsync(ResetPasswordRequestDto request)
    {
        var otpCode = _httpContextAccessor.HttpContext.Items["OtpToken"] as OtpCode ?? new OtpCode();

        if (otpCode.Purpose != OtpPurposes.reset_password.ToString())
        {
            throw new GlobalException(
                code: CustomerAccountErrorCode.Unauthorized,
                message: L[CustomerAccountErrorCode.Unauthorized],
                statusCode: HttpStatusCode.Unauthorized
            );
        }

        using (_unitOfWork.BeginTransaction())
        {
            try
            {
                var customerAccount = new CustomerAccount();
                var query = _unitOfWork.GenericRepository<CustomerAccount>();

                if (otpCode.RecipientTypes == RecipientTypes.email.ToString())
                {
                    customerAccount = await query.GetQueryable().Where(x => x.Status == CustomerStatuses.Active && x.Email == otpCode.Recipient && !ExcludedStatuses.Contains(x.Status)).FirstOrDefaultAsync();
                }
                else if (otpCode.RecipientTypes == RecipientTypes.phone.ToString())
                {
                    customerAccount = await query.GetQueryable().Where(x => x.Status == CustomerStatuses.Active && x.PhoneNumber == otpCode.Recipient && !ExcludedStatuses.Contains(x.Status)).FirstOrDefaultAsync();
                }

                if (customerAccount == null || customerAccount.Id == Guid.Empty)
                {
                    throw new GlobalException(code: CustomerAccountErrorCode.AccountNotFound,
                                 message: L[CustomerAccountErrorCode.AccountNotFound],
                                 statusCode: HttpStatusCode.NotFound);
                }

                otpCode.Status = OtpStatuses.Used;
                _unitOfWork.GenericRepository<OtpCode>().Update(otpCode);

                customerAccount.PasswordHash = _passwordHasher.Hash(customerAccount, request.NewPassword!);
                _unitOfWork.GenericRepository<CustomerAccount>().Update(customerAccount);
                await _unitOfWork.SaveChangeAsync();

                _unitOfWork.Commit();

                return new BaseResponse();
            }
            catch (GlobalException)
            {
                _unitOfWork.Rollback();
                throw;
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

    public async Task<BaseResponse> ChangePasswordAsync(Guid id, ChangePasswordRequestDto request)
    {
        var customerAccount = await _unitOfWork.GenericRepository<CustomerAccount>().GetQueryable().FirstOrDefaultAsync(x => x.Id == id && x.Status == CustomerStatuses.Active);
        if (customerAccount == null)
        {
            throw new GlobalException(code: CustomerAccountErrorCode.AccountNotFound,
                                      message: L[CustomerAccountErrorCode.AccountNotFound],
                                      statusCode: HttpStatusCode.NotFound);
        }
        if (!_passwordHasher.Verify(customerAccount, request.OldPassword!, customerAccount.PasswordHash))
        {
            throw new GlobalException(code: CustomerAccountErrorCode.InvalidCredentials,
                                      message: L[CustomerAccountErrorCode.InvalidCredentials],
                                      statusCode: HttpStatusCode.Unauthorized);
        }
        customerAccount.PasswordHash = _passwordHasher.Hash(customerAccount, request.NewPassword!);
        _unitOfWork.GenericRepository<CustomerAccount>().Update(customerAccount);
        await _unitOfWork.SaveChangeAsync();

        return new BaseResponse();
    }

    public async Task<ProfileDto> GetProfileAsync(Guid id)
    {
        var customerAccount = await _unitOfWork.GenericRepository<CustomerAccount>().GetQueryable().AsNoTracking()
            .Where(x => x.Id == id && !ExcludedStatuses.Contains(x.Status))
            .Include(x => x.Customer).FirstOrDefaultAsync();

        if (customerAccount == null)
        {
            throw new GlobalException(code: CustomerAccountErrorCode.AccountNotFound,
                                      message: L[CustomerAccountErrorCode.AccountNotFound],
                                      statusCode: HttpStatusCode.NotFound);
        }
        var profileDto = new ProfileDto
        {
            Id = customerAccount.Id,
            PhoneNumber = customerAccount.PhoneNumber,
            FirstName = customerAccount.FirstName,
            LastName = customerAccount.LastName,
            Email = customerAccount.Email,
            DOB = customerAccount.Customer == null ? null : customerAccount.Customer!.DOB,
            Gender = customerAccount.Gender,
            Address = customerAccount.Customer == null ? null : customerAccount.Customer!.Address,
        };

        // Lấy số dư trên ví điểm
        var customerPoint = customerAccount.Customer != null ? await GetCustomerPointWalletAsync(customerAccount.Customer.Id) : null;

        if (customerPoint != null)
        {
            profileDto.PointWallet = new PointWalletDto
            {
                Id = customerPoint.Id,
                TotalAccumulated = customerPoint.TotalAccumulated,
                TotalUsed = customerPoint.TotalUsed,
                CurrentBalance = customerPoint.CurrentBalance,
                HoldPoint = customerPoint.HoldPoint
            };
        }

        // Kiểm tra trạng thái tài khoản chưa xác thực thì trả về thông tin yêu cầu xác thực
        // Nếu tài khoản đã xác thực, kiểm tra xem đã map đến Customer chưa
        if (customerAccount.Status == CustomerStatuses.Pending || customerAccount.CustomerId == null || customerAccount.CustomerId == Guid.Empty)
        {
            var meta = await BuildMapAccountMetaAsync(customerAccount);
            profileDto.Meta = meta.Item1;
        }

        return profileDto;
    }

    public async Task<ProfileRequestDto> UpdateProfileAsync(Guid id, ProfileRequestDto request)
    {
        using (_unitOfWork.BeginTransaction())
        {
            try
            {
                var customerAccount = await _unitOfWork.GenericRepository<CustomerAccount>().GetQueryable()
                    .Include(x => x.Customer)
                    .FirstOrDefaultAsync(x => x.Id == id && x.Status == CustomerStatuses.Active);

                if (customerAccount == null)
                {
                    throw new GlobalException(code: CustomerAccountErrorCode.AccountNotFound,
                        message: L[CustomerAccountErrorCode.AccountNotFound],
                        statusCode: HttpStatusCode.NotFound);
                }

                if (customerAccount.EmailVerifiedAt.HasValue && request.Email != customerAccount.Email)
                {
                    throw new GlobalException(code: CustomerAccountErrorCode.EmailAlreadyVerified,
                                              message: L[CustomerAccountErrorCode.EmailAlreadyVerified],
                                              statusCode: HttpStatusCode.BadRequest);
                }

                if (!string.IsNullOrEmpty(request.Email))
                {
                    var emailExists = await _unitOfWork.GenericRepository<CustomerAccount>()
                        .GetQueryable()
                        .AsNoTracking()
                        .AnyAsync(x => x.Email == request.Email && x.Id != customerAccount.Id && x.Status != CustomerStatuses.Deleted);

                    if (emailExists)
                    {
                        throw new GlobalException(
                            code: CustomerAccountErrorCode.EmailAlreadyUsed,
                            message: L[CustomerAccountErrorCode.EmailAlreadyUsed],
                            statusCode: HttpStatusCode.BadRequest);
                    }
                }

                customerAccount.Email = request.Email;
                customerAccount.FirstName = request.FirstName;
                customerAccount.LastName = request.LastName;
                customerAccount.Gender = request.Gender;

                if (customerAccount.Customer != null)
                {
                    customerAccount.Customer!.Email = request.Email ?? "";
                    customerAccount.Customer!.NormalizedEmail = request.Email!.ToUpper();
                    customerAccount.Customer!.FirstName = request.FirstName;
                    customerAccount.Customer!.LastName = request.LastName;
                    customerAccount.Customer!.DOB = request.DOB;
                    customerAccount.Customer!.Gender = request.Gender;

                    var location = await _locationServiceShared.GetLocations(new LocationRequestDto()
                    {
                        ProvinceId = request.Address.ProvinceId,
                        DistrictId = request.Address.DistrictId,
                        WardId = request.Address.WardId,
                        Locate = request.Address.Locate
                    });

                    customerAccount.Customer.Address ??= new Address();
                    var addressSpecifically = string.Empty;
                    if (location.Ward != null) addressSpecifically = $"{location.Ward}";
                    if (location.District != null) addressSpecifically += $", {location.District}";
                    if (location.Province != null) addressSpecifically += $", {location.Province}";
                    if (!string.IsNullOrEmpty(request.Address.Street))
                    {
                        customerAccount.Customer.Address!.JoinedName = request.Address.Street;
                        if (!string.IsNullOrEmpty(addressSpecifically))
                        {
                            customerAccount.Customer.Address.JoinedName += ", " + addressSpecifically;
                        }
                    }
                    else
                    {
                        customerAccount.Customer.Address!.JoinedName = addressSpecifically;
                    }

                    customerAccount.Customer!.Address!.WardId = request.Address.WardId;
                    customerAccount.Customer!.Address!.ProvinceId = request.Address.ProvinceId;
                    customerAccount.Customer!.Address!.DistrictId = request.Address.DistrictId;

                    customerAccount.Customer!.Address!.WardName = location.Ward;
                    customerAccount.Customer!.Address!.DistrictName = location.District;
                    customerAccount.Customer!.Address!.ProvinceName = location.Province;
                    customerAccount.Customer!.Address!.Locate = location.Locate;
                    customerAccount.Customer!.Address!.Street = request.Address.Street;
                    customerAccount.Customer!.NormalizeAddress = NormalizeString.ConvertNormalizeString(customerAccount.Customer!.Address!.JoinedName!);
                    customerAccount.Customer!.NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{customerAccount.Customer.FirstName} {customerAccount.Customer.LastName} {customerAccount.Customer.CompanyName}");
                    _unitOfWork.GenericRepository<Customer>().Update(customerAccount.Customer!);
                }

                _unitOfWork.GenericRepository<CustomerAccount>().Update(customerAccount);

                await _unitOfWork.SaveChangeAsync();
                _unitOfWork.Commit();

                return request;
            }
            catch (GlobalException)
            {
                _unitOfWork.Rollback();
                throw;
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

    public async Task<BaseResponse> DeleteAccountAsync(Guid id)
    {
        var customerAccount = await _unitOfWork.GenericRepository<CustomerAccount>().GetQueryable().FirstOrDefaultAsync(x => x.Id == id && x.Status == CustomerStatuses.Active);
        if (customerAccount == null)
        {
            throw new GlobalException(code: CustomerAccountErrorCode.AccountNotFound,
                                      message: L[CustomerAccountErrorCode.AccountNotFound],
                                      statusCode: HttpStatusCode.NotFound);
        }

        customerAccount.Status = CustomerStatuses.Deleted;
        customerAccount.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _unitOfWork.GenericRepository<CustomerAccount>().Update(customerAccount);
        await _unitOfWork.SaveChangeAsync();

        return new BaseResponse();
    }

    public async Task<CustomerAccount?> GetActiveAccountAsync(Guid customerAccountId)
    {
        return await _unitOfWork.GenericRepository<CustomerAccount>().GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == customerAccountId && !ExcludedStatuses.Contains(x.Status));
    }

    public async Task<BaseResponse> RequestDeleteAccountAsync(SignInRequestDto request)
    {
        var customerAccount = await _unitOfWork.GenericRepository<CustomerAccount>().GetQueryable().FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber && x.Status != CustomerStatuses.Deleted);
        if (customerAccount == null)
        {
            throw new GlobalException(code: CustomerAccountErrorCode.InvalidCredentials,
                message: L[CustomerAccountErrorCode.InvalidCredentials],
                statusCode: HttpStatusCode.Unauthorized);
        }

        if (!_passwordHasher.Verify(customerAccount, request.Password!, customerAccount.PasswordHash))
        {
            throw new GlobalException(code: CustomerAccountErrorCode.InvalidCredentials,
                message: L[CustomerAccountErrorCode.InvalidCredentials],
                statusCode: HttpStatusCode.Unauthorized);
        }

        customerAccount.Status = CustomerStatuses.Deleted;
        customerAccount.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _unitOfWork.GenericRepository<CustomerAccount>().Update(customerAccount);
        await _unitOfWork.SaveChangeAsync();

        return new BaseResponse();
    }

    private string GenerateCustomerAccountJwtToken(CustomerAccountTokenDto payload)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(CustomerAccountConst.CLAIM_CUSTOMER_ID_KEY, payload.CustomerAccountId.ToString()),
            new Claim(CustomerAccountConst.CLAIM_CUSTOMER_ACCOUNT_ID_KEY, payload.CustomerAccountId.ToString()),
            // new Claim(ClaimTypes.NameIdentifier, payload.CustomerAccountId.ToString()),
            new Claim(CustomerAccountConst.CLAIM_TYPE_KEY, payload.Type)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: payload.ExpiresIn,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<(MetaDto, Customer?)> BuildMapAccountMetaAsync(CustomerAccount customerAccount)
    {
        if (customerAccount.Status == CustomerStatuses.Pending)
        {
            return new(new MetaDto
            {
                Requirements = new RequirementDto
                {
                    NextStep = RequirementsNextStep.verify_account.ToString(),
                    Message = L[CustomerAccountErrorCode.VerifyAccount],
                    Action = new RequirementActionDto
                    {
                        Params = new RequirementParamsDto
                        {
                            ReferenceTypes = ReferenceTypes.CustomerAccount.ToString(),
                            ReferenceId = customerAccount.Id.ToString(),
                            Purpose = OtpPurposes.verify_account.ToString()
                        }
                    }
                }
            }, null);
        }

        var customer = await _unitOfWork.GenericRepository<Customer>().GetQueryable()
            .FirstOrDefaultAsync(x => !string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.Contains(customerAccount.PhoneNumber) && !x.DeletedAt.HasValue);

        if (customer == null) return new(new MetaDto(), null);

        var meta = new MetaDto
        {
            Requirements = new RequirementDto
            {
                NextStep = RequirementsNextStep.map_account.ToString(),
                Message = L[CustomerAccountErrorCode.MapAccount],
                Action = new RequirementActionDto
                {
                    Params = new RequirementParamsDto
                    {
                        CustomerId = customer.Id.ToString(),
                    }
                },
                Data = new ProfileCustomerDto
                {
                    PhoneNumber = customer.PhoneNumber,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    DOB = customer.DOB,
                    Gender = customer.Gender,
                    Address = customer.Address,
                }
            }
        };

        return new(meta, customer);
    }

    public async Task<CustomerPoint> GetCustomerPointWalletAsync(Guid customerId)
    {
        var existingPoint = await _unitOfWork.GenericRepository<CustomerPoint>().GetQueryable()
            .FirstOrDefaultAsync(x => x.CustomerId == customerId);
        if (existingPoint != null)
            return existingPoint;

        var customerPoint = new CustomerPoint
        {
            CustomerId = customerId,
        };
        await _unitOfWork.GenericRepository<CustomerPoint>().AddAsync(customerPoint);
        await _unitOfWork.SaveChangeAsync();
        return customerPoint;
    }

    public async Task<CustomerAccount> GetCustomerAccountAsync(Guid id)
    {
        var customerAccount = await _unitOfWork.GenericRepository<CustomerAccount>().GetQueryable()
            .Include(x => x.Customer)
            .Where(x => x.CustomerId == id)
            .FirstOrDefaultAsync();
        if (customerAccount == null)
        {
            throw new GlobalException(code: CustomerAccountErrorCode.AccountNotFound,
                message: L[CustomerAccountErrorCode.AccountNotFound],
                statusCode: HttpStatusCode.NotFound);
        }
        return customerAccount;
    }

    public async Task ResetPasswordAsync(ResetPasswordRequestDto request, Guid id)
    {
        if (request.NewPassword != request.PasswordConfirm)
        {
            throw new GlobalException(code: CustomerAccountErrorCode.PasswordNotMatched,
                message: L[CustomerAccountErrorCode.PasswordNotMatched],
                statusCode: HttpStatusCode.BadRequest);
        }

        var customerAccount = await _unitOfWork.GenericRepository<CustomerAccount>().GetQueryable()
                                                    .FirstOrDefaultAsync(x => x.Id == id && x.Status != CustomerStatuses.Deleted);

        if (customerAccount == null)
        {
            throw new GlobalException(code: CustomerAccountErrorCode.AccountNotFound,
                message: L[CustomerAccountErrorCode.AccountNotFound],
                statusCode: HttpStatusCode.NotFound);
        }

        customerAccount.PasswordHash = _passwordHasher.Hash(customerAccount, request.NewPassword!);
        _unitOfWork.GenericRepository<CustomerAccount>().Update(customerAccount);
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task UpdateStatusAccountAsync(Guid id, UpdateStatusDto dto)
    {
        var customerAccount = await _unitOfWork.GenericRepository<CustomerAccount>().GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (customerAccount == null)
        {
            throw new GlobalException(code: CustomerAccountErrorCode.AccountNotFound,
                message: L[CustomerAccountErrorCode.AccountNotFound],
                statusCode: HttpStatusCode.NotFound);
        }

        if (dto.Status == customerAccount.Status)
        {
            return;
        }

        if (customerAccount.Status == CustomerStatuses.Deleted)
        {
            throw new GlobalException(code: CustomerAccountErrorCode.NotAllowUpdate,
                message: L[CustomerAccountErrorCode.NotAllowUpdate],
                statusCode: HttpStatusCode.BadRequest);
        }

        if (dto.Status != CustomerStatuses.Active && dto.Status != CustomerStatuses.InActive)
        {
            throw new GlobalException(code: CustomerAccountErrorCode.StatusNotAllowed,
                message: L[CustomerAccountErrorCode.StatusNotAllowed],
                statusCode: HttpStatusCode.BadRequest);
        }
        customerAccount.Status = dto.Status;
        _unitOfWork.GenericRepository<CustomerAccount>().Update(customerAccount);
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task<BaseResponse> RestoreDeletedAccountByAdminAsync(Guid id)
    {
        var customerAccount = await _unitOfWork.GenericRepository<CustomerAccount>()
            .GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (customerAccount == null)
        {
            throw new GlobalException(
                code: CustomerAccountErrorCode.AccountNotFound,
                message: L[CustomerAccountErrorCode.AccountNotFound],
                statusCode: HttpStatusCode.NotFound);
        }

        var accountStatus = customerAccount.Status;
        if (accountStatus != CustomerStatuses.Deleted)
        {
            if (accountStatus == CustomerStatuses.Pending)
            {
                throw new GlobalException(code: CustomerAccountErrorCode.AccountNotFound,
                                 message: L[CustomerAccountErrorCode.AccountNotFound],
                                 statusCode: HttpStatusCode.NotFound);
            }
            if (accountStatus == CustomerStatuses.Banned)
            {
                throw new GlobalException(code: CustomerAccountErrorCode.AccountBanned,
                    message: L[CustomerAccountErrorCode.AccountBanned],
                    statusCode: HttpStatusCode.BadRequest);
            }
            if (accountStatus == CustomerStatuses.InActive)
            {
                throw new GlobalException(code: CustomerAccountErrorCode.UnverifiedAccount,
                    message: L[CustomerAccountErrorCode.UnverifiedAccount],
                    statusCode: HttpStatusCode.BadRequest);
            }
            if (accountStatus == CustomerStatuses.Active)
            {
                throw new GlobalException(code: CustomerAccountErrorCode.AlreadyActive,
                    message: L[CustomerAccountErrorCode.AlreadyActive],
                    statusCode: HttpStatusCode.BadRequest);
            }
        }
        var deletedDuration = DateTime.UtcNow - customerAccount.DeletedAt;
        if (deletedDuration > TimeSpan.FromDays(30))
        {
            throw new GlobalException(code: CustomerAccountErrorCode.AccountNotFound,
                                 message: L[CustomerAccountErrorCode.AccountNotFound],
                                 statusCode: HttpStatusCode.NotFound);
        }

        customerAccount.Status = CustomerStatuses.Active;
        customerAccount.DeletedAt = null;
        _unitOfWork.GenericRepository<CustomerAccount>().Update(customerAccount);
        await _unitOfWork.SaveChangeAsync();
        return new BaseResponse();
    }

}
