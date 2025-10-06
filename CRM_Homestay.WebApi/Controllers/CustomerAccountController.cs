using CRM_Homestay.App.Attributes;
using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Coupons;
using CRM_Homestay.Contract.CustomerAccounts;
using CRM_Homestay.Contract.Users;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Entity.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Homestay.App.Controllers
{
    /// <summary>
    /// CustomerAccountController
    /// </summary>
    [ApiController]
    [Route("api/customer-accounts/")]
    public class CustomerAccountController : BaseController
    {
        private readonly ICustomerAccountService _customerAccountService;
        private readonly ICouponService _couponService;

        /// <summary>
        /// CustomerAccountController init
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="customerAccountService"></param>
        /// <param name="couponService"></param>
        public CustomerAccountController(IHttpContextAccessor httpContextAccessor, ICustomerAccountService customerAccountService, ICouponService couponService) : base(httpContextAccessor)
        {
            _customerAccountService = customerAccountService;
            _couponService = couponService;
        }

        /// <summary>
        /// Đăng ký
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("sign-up")]
        [AllowAnonymous]
        public async Task<SignUpResponseDto> SignUpAsync(SignUpRequestDto request)
        {
            return await _customerAccountService.SignUpAsync(request);
        }

        /// <summary>
        /// Verify
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("verify")]
        [UseOtpToken]
        public async Task<VerifyAccountResponseDto> VerifyAsync()
        {
            return await _customerAccountService.VerifyAsync();
        }

        /// <summary>
        /// Map with customer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("map-with-customer")]
        [CustomerAuthorize(allowInActive: true)]
        public async Task<BaseResponse> MapWithCustomerAsync(MapCustomerRequestDto request)
        {
            var customerAccountId = GetCurrentCustomerAccountId();
            return await _customerAccountService.MapWithCustomerAsync(customerAccountId, request);
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<SignUpResponseDto> SignInAsync(SignInRequestDto request)
        {
            return await _customerAccountService.SignInAsync(request);
        }

        /// <summary>
        /// Quên mật khẩu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("reset-password")]
        [UseOtpToken]
        public async Task<BaseResponse> ForgotPasswordAsync(ResetPasswordRequestDto request)
        {
            return await _customerAccountService.ForgotPasswordAsync(request);
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("change-password")]
        [CustomerAuthorize(allowInActive: true)]
        public async Task<BaseResponse> ChangePasswordAsync(ChangePasswordRequestDto request)
        {
            var customerAccountId = GetCurrentCustomerAccountId();
            return await _customerAccountService.ChangePasswordAsync(customerAccountId, request);
        }

        /// <summary>
        /// Get profile
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-profile")]
        [CustomerAuthorize(allowInActive: true)]
        public async Task<ProfileDto> GetProfileAsync()
        {
            var customerAccountId = GetCurrentCustomerAccountId();
            return await _customerAccountService.GetProfileAsync(customerAccountId);
        }

        /// <summary>
        /// Update profile
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("update")]
        [CustomerAuthorize]
        public async Task<ProfileRequestDto> UpdateProfileAsync(ProfileRequestDto request)
        {
            var customerAccountId = GetCurrentCustomerAccountId();
            return await _customerAccountService.UpdateProfileAsync(customerAccountId, request);
        }

        /// <summary>
        /// Xóa tài khoản
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [CustomerAuthorize]
        public async Task<BaseResponse> DeleteAccountAsync()
        {
            var customerAccountId = GetCurrentCustomerAccountId();
            return await _customerAccountService.DeleteAccountAsync(customerAccountId);
        }

        /// <summary>
        /// Yêu cầu xóa tài khoản
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("request-delete-account")]
        public async Task<BaseResponse> RequestDeleteAccountAsync(SignInRequestDto request)
        {
            return await _customerAccountService.RequestDeleteAccountAsync(request);
        }

        /// <summary>
        /// Lấy thông tin tài khoản khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/account")]
        [Authorize(Roles = RoleCodes.ADMIN)]
        public async Task<CustomerAccount> GetCustomerAccountAsync(Guid id)
        {
            return await _customerAccountService.GetCustomerAccountAsync(id);
        }

        /// <summary>
        /// Đặt lại mật khẩu cho tài khoản khách hàng ở phía admin
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("reset-password")]
        [Authorize(Roles = RoleCodes.ADMIN)]
        public async Task ResetPasswordAsync([FromBody] ResetPasswordRequestDto request, [FromQuery] Guid id)
        {
            await _customerAccountService.ResetPasswordAsync(request, id);
        }

        /// <summary>
        /// Cập nhật trạng thái tài khoản khách hàng ở phía admin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("update-status")]
        [Authorize(Roles = RoleCodes.ADMIN)]
        public async Task UpdateStatusAccountAsync(Guid id, [FromBody] UpdateStatusDto input)
        {
            await _customerAccountService.UpdateStatusAccountAsync(id, input);
        }

        /// <summary>
        /// Xóa tài khoản khách hàng ở phía admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = RoleCodes.ADMIN)]
        public async Task<BaseResponse> DeleteAccountAsync(Guid id)
        {
            return await _customerAccountService.DeleteAccountAsync(id);
        }

        /// <summary>
        /// Lấy ra list mã giảm giá mà khách hàng có thể sử dụng
        /// </summary>
        /// <returns></returns>
        [HttpGet("coupons/available")]
        public async Task<List<CouponDto>> GetListForCustomer()
        {
            var customerAccount = GetCurrentCustomerAccount();
            return await _couponService.GetAvailableCouponsForCustomerAsync(customerAccount.CustomerId ?? Guid.Empty);
        }

        /// <summary>
        /// Khôi phục tài khoản by Admin
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin, office_staff")]
        [Route("restore-customer-account/{id}")]
        public async Task<BaseResponse> RestoreCustomerAccountAsync(Guid id)
        {
            return await _customerAccountService.RestoreDeletedAccountByAdminAsync(id);
        }
    }
}

