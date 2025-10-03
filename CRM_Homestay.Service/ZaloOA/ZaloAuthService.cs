using System.Net.Http.Json;
using CRM_Homestay.Contract.ZaloOA;
using CRM_Homestay.Shared.Helpers;
using Microsoft.Extensions.Configuration;

namespace CRM_Homestay.Service.WarehouseTransfers
{
    public class ZaloAuthService : IZaloAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ZaloAuthService(IConfiguration config)
        {
            _httpClient = new HttpClient();
            _config = config;
        }

        public async Task<TokenResponseDto?> GetAccessTokenAsync(string code, string? codeVerifier = null)
        {
            var appId = _config["Zalo:AppId"];
            var secretKey = _config["Zalo:SecretKey"];

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["app_id"] = appId!,
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["code_verifier"] = codeVerifier!
            });

            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.zaloapp.com/v4/oa/access_token");
            request.Content = content;
            request.Headers.Add("secret_key", secretKey!);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        }

        public async Task<TokenResponseDto?> RefreshAccessTokenAsync(string refreshToken)
        {
            var appId = _config["Zalo:AppId"];
            var secretKey = _config["Zalo:SecretKey"];

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["app_id"] = appId!,
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            });

            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.zaloapp.com/v4/oa/access_token");
            request.Content = content;
            request.Headers.Add("secret_key", secretKey!);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        }

        /// <summary>
        /// Tạo code_verifier + code_challenge để redirect sang Zalo.
        /// </summary>
        public async Task<string> GenerateAuthorizationRequest()
        {
            var appId = _config["Zalo:AppId"];
            var redirectUri = _config["Zalo:RedirectUri"];

            var codeVerifier = await Task.Run(() => PkceHelper.GenerateCodeVerifier());
            var codeChallenge = await Task.Run(() => PkceHelper.CreateCodeChallenge(codeVerifier));

            var state = Guid.NewGuid().ToString("N");

            var authUrl = $"https://oauth.zaloapp.com/v4/oa/permission?" +
                          $"app_id={appId}" +
                          $"&redirect_uri={Uri.EscapeDataString(redirectUri!)}" +
                          $"&code_challenge={codeChallenge}" +
                            $"&code_challenge_method=S256" +
                          $"&state={state}";

            return authUrl;
        }

    }
}
