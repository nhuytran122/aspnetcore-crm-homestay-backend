using AutoMapper;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Localization;
using CRM_Homestay.Database.Repositories;

namespace CRM_Homestay.Service;

public class BaseService
{
    protected IMapper ObjectMapper { get; }
    protected IUnitOfWork _unitOfWork { get; }
    protected ILocalizer L { get; }
    private bool IsQualifiedRequest { get; set; } = true;
    public BaseService(IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l)
    {
        ObjectMapper = mapper;
        _unitOfWork = unitOfWork;
        L = l;
    }

    public void ThrowGlobalException(GlobalException exception)
    {
        IsQualifiedRequest = false;
        throw exception;
    }
}

