using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CRM_Homestay.Core.Models;

public class Address
{
     public int ProvinceId { get; set; }
     public string? ProvinceName { get; set; }
     public int DistrictId { get; set; }
     public string? DistrictName { get; set; }
     public int WardId { get; set; }
     public string? WardName { get; set; }
     public string? Street { get; set; }
     public string? JoinedName { get; set; }
     public string? Locate { get; set; }
}

public class CreateUpdateAddressDto
{
     public int ProvinceId { get; set; }
     public int DistrictId { get; set; }
     public int WardId { get; set; }
     public string? Street { get; set; }
     public string? Locate { get; set; }
}
