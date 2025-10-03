namespace CRM_Homestay.Contract.ZaloOA
{
    public class TokenResponseDto
    {
        public string access_token { get; set; } = "";
        public string refresh_token { get; set; } = "";
        public string expires_in { get; set; } = "";
    }
}