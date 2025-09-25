using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;
using Microsoft.AspNetCore.Http;

namespace CRM_Homestay.Contract.Branches
{
    public class CreateUpdateBranchDto
    {
        public string Name { get; set; } = string.Empty;
        public CreateUpdateAddressDto Address { get; set; } = new CreateUpdateAddressDto();
        public string PhoneNumber { get; set; } = string.Empty;
        public string GatePassword { get; set; } = string.Empty;
        public bool IsMainBranch { get; set; } = false;
        public BranchStatuses Status { get; set; } = BranchStatuses.Active;
        public IFormFile? Image { get; set; }
    }
}
