namespace CRM_Homestay.Contract.ZaloOA
{
    public interface IZaloAuthService
    {
        Task<TokenResponseDto?> GetAccessTokenAsync(string code, string? codeVerifier = null);
        Task<TokenResponseDto?> RefreshAccessTokenAsync(string refreshToken);
        public Task<string> GenerateAuthorizationRequest();
    }
}
