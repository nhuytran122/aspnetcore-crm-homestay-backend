using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.CustomerGroups;

public class CustomerGroupFilterDto : IPagedResultRequest
{
    public bool? IsActive { get; set; }
    private string? _text;

    public string? Text
    {
        get
        {
            return _text;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                _text = value.Trim().ToLower();
            }
            else
            {
                _text = value;
            }
        }
    }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}