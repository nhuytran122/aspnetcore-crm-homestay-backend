using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.BranchInventories;
using CRM_Homestay.Entity.HomestayMaintenances;
using CRM_Homestay.Entity.ImportProducts;
using CRM_Homestay.Entity.Medias;
using CRM_Homestay.Entity.Rooms;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM_Homestay.Entity.Branches
{
    public class Branch : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        [Column(TypeName = "json")]
        public Address? Address { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public BranchStatuses Status { get; set; }
        public string GatePassword { get; set; } = string.Empty;
        public bool IsMainBranch { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? NormalizeFullInfo { get; set; }
        public string? NormalizeAddress { get; set; }
        public Guid? MediaId { get; set; }

        public List<ImportProduct>? ImportProducts { get; set; }
        public List<BranchInventory>? BranchInventories { get; set; }
        public List<Room>? Rooms { get; set; }
        public List<HomestayMaintenance>? HomestayMaintenances { get; set; }
        public BaseMedia? Media { get; set; }
        
    }
    public class BranchInfo
    {
        public Guid BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
    }
}
