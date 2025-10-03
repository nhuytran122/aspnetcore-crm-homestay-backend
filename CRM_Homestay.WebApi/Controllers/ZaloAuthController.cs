using CRM_Homestay.Contract.ZaloOA;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Homestay.App.Controllers
{
    /// <summary>
    /// Controller xử lý luồng OAuth cho Zalo Official Account.
    /// </summary>
    [Route("zalo/oauth")]
    [ApiController]
    public class ZaloAuthController : ControllerBase
    {
        private readonly IZaloAuthService _zaloAuth;

        /// <summary>
        /// Khởi tạo controller ZaloAuth.
        /// </summary>
        /// <param name="zaloAuth">Service xử lý xác thực Zalo OA.</param>
        public ZaloAuthController(IZaloAuthService zaloAuth)
        {
            _zaloAuth = zaloAuth;
        }

        /// <summary>
        /// Callback được gọi sau khi người dùng xác thực với Zalo OA.
        /// </summary>
        /// <param name="code">Mã code Zalo trả về để lấy access_token.</param>
        /// <param name="oa_id">ID của Official Account (OA) tương ứng.</param>
        /// <returns>Access token và thông tin liên quan.</returns>
        /// <remarks>
        /// Ví dụ request:
        /// <para>GET /zalo/oauth/callback?code=xxx&amp;oa_id=yyy</para>
        /// </remarks>
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string oa_id)
        {
            var token = await _zaloAuth.GetAccessTokenAsync(code);

            // TODO: Lưu token, refresh_token vào DB (gắn theo oa_id)
            return Ok(token);
        }

        /// <summary>
        /// Làm mới access token bằng refresh token.
        /// </summary>
        /// <param name="refreshToken">Refresh token đã lưu trong DB.</param>
        /// <returns>Access token mới và thông tin liên quan.</returns>
        /// <remarks>
        /// Ví dụ request:
        /// <para>POST /zalo/oauth/refresh?refreshToken=zzz</para>
        /// </remarks>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromQuery] string refreshToken)
        {
            var token = await _zaloAuth.RefreshAccessTokenAsync(refreshToken);
            return Ok(token);
        }
    }
}
