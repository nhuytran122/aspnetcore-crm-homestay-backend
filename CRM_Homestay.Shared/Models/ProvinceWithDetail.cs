namespace CRM_Homestay.Core.Models;

public class ProvinceWithDetail
{
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }
    public string Division_Type { get; set; } = string.Empty;
    public List<DistrictWithDetail>? Districts { get; set; }
}

public class DistrictWithDetail
{
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }
    public string Division_Type { get; set; } = string.Empty;
    public List<Ward>? Wards { get; set; }
}

public class Ward
{
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }
    public string Division_Type { get; set; } = string.Empty;
}