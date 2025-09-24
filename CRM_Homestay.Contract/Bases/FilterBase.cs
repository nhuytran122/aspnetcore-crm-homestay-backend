namespace CRM_Homestay.Contract.Bases;

public class FilterBase
{
    private DateTime? _startDate { get; set; }
    private DateTime? _endDate { get; set; }

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

    public DateTime? StartDate
    {
        get { return _startDate; }
        set
        {
            if (value.HasValue)
            {
                _startDate = DateTime.SpecifyKind(value.Value, DateTimeKind.Unspecified);
            }
            else
            {
                _startDate = null;
            }
        }

    }

    public DateTime? EndDate
    {
        get { return _endDate; }
        set
        {
            if (value.HasValue)
            {
                _endDate = DateTime.SpecifyKind(value.Value, DateTimeKind.Unspecified);
            }
            else
            {
                _endDate = null;
            }
        }
    }

    public virtual bool IsValidFilter()
    {
        if (StartDate.HasValue && EndDate.HasValue)
        {
            if (StartDate.Value.Date > EndDate.Value.Date)
            {
                return false;
            }
        }
        return true;
    }
}