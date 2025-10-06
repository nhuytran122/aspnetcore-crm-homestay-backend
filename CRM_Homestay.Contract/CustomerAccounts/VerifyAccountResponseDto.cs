namespace CRM_Homestay.Contract.CustomerAccounts;

public class VerifyAccountResponseDto
{
    public bool Success { get; set; } = true;
    public MetaDto? Meta { get; set; }
}
