
using CRM_Homestay.Contract.Locations;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;

namespace CRM_Homestay.Contract.Branches
{
    public class BranchDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Address Address { get; set; } = new Address();
        public LocationDto LocationName { get; set; } = new LocationDto();
        public DateTime CreationTime { get; set; }
        public string? PhoneNumber { get; set; }
        public BranchStatuses Status { get; set; }
        public bool? IsMainBranch { get; set; }

        public string? GatePassword { get; set; }
        public string? MediaUrl { get; set; }
    }
}
