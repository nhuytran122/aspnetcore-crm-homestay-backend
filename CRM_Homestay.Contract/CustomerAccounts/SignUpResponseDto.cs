namespace CRM_Homestay.Contract.CustomerAccounts;

public class SignUpResponseDto
{
    public Guid Id { get; set; }
    public string AccessToken { get; set; } = "";
}
