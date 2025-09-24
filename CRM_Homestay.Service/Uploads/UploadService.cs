using AutoMapper;
using CRM_Homestay.Contract.Medias;
using CRM_Homestay.Contract.Uploads;
using CRM_Homestay.Core.Helpers;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Medias;
using CRM_Homestay.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace CRM_Homestay.Service.Uploads;

public class UploadService : BaseService, IUploadService
{
    private readonly IConfiguration _configuration;
    private string RootPath { get; set; }
    public string RootURL { get; set; }
    private string Key { get; set; } = "static-files";

    public UploadService(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l, IConfiguration configuration, IHostEnvironment environment) : base(unitOfWork, mapper, l)
    {
        _configuration = configuration;
        RootPath = Path.Combine(environment.ContentRootPath, "wwwroot");
        try
        {
            RootURL = $"{Path.Combine(httpContextAccessor.HttpContext.Request.Scheme + "://", httpContextAccessor.HttpContext.Request.Host.Value)}/{Key}";
        }
        catch (Exception)
        {
            var host = _configuration.GetValue<string>("Environment:Host") ?? "localhost";
            RootURL = $"{host}/{Key}";
        }
    }

    public async Task<MediaDto> UploadAvatar(IFormFile file)
    {
        string pathBase = Path.Combine(RootPath, _configuration["Media:Avatars"]!);
        string pathUrl = Path.Combine(RootURL, _configuration["Media:Avatars"]!);
        var res = await FileHelper.UploadFile(L, file, pathBase, new List<string>() { ".jpg", ".jpeg", ".png" }, pathUrl);
        return new MediaDto() { Url = res.Url, Extension = res.Extension, Name = res.Name };
    }

    public async Task<string> UploadProduct(IFormFile file)
    {
        string pathBase = Path.Combine(RootPath, _configuration["Media:Products"]!);
        string pathUrl = Path.Combine(RootURL, _configuration["Media:Products"]!);
        var res = await FileHelper.UploadFile(L, file, pathBase, new List<string>() { ".jpg", ".jpeg", ".png" }, pathUrl);
        return res.Url!.Replace(RootURL, "");
    }

    public string GetRootUrl()
    {
        return RootURL;
    }

    public string GetRootPath()
    {
        return RootPath;
    }

    public async Task<List<string>> UploadPaymentImg(List<IFormFile> files)
    {
        string pathBase = Path.Combine(RootPath, _configuration["Media:OrderPayments"]!);
        string pathUrl = Path.Combine(RootURL, _configuration["Media:OrderPayments"]!);
        var urlsList = new List<string>();
        foreach (var item in files)
        {
            var res = await FileHelper.UploadFile(L, item, pathBase, new List<string>() { ".jpg", ".jpeg", ".png" }, pathUrl);
            urlsList.Add(res.Url!.Replace(RootURL, ""));
        }
        return urlsList;
    }

    public bool DeleteImage(string imageUrl)
    {
        try
        {
            string filePath = Path.Combine(RootPath, imageUrl.TrimStart('/'));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<BaseMedia>> UploadImgs(List<IFormFile> files, string path)
    {
        string pathBase = Path.Combine(RootPath, _configuration[path]!);
        string pathUrl = Path.Combine(RootURL, _configuration[path]!);
        var response = new List<BaseMedia>();
        foreach (var item in files)
        {
            var res = await FileHelper.UploadFile(L, item, pathBase, new List<string>() { ".jpg", ".jpeg", ".png" }, pathUrl);

            response.Add(new BaseMedia
            {
                Extension = res.Extension,
                FileName = res.Name,
                FileNameUpload = res.NameUpload,
                FilePath = res.Url.Replace(RootURL, ""),
                Type = "Image"
            });
        }
        return response;
    }

    public async Task<BaseMedia> UploadImg(IFormFile file, string path)
    {
        string pathBase = Path.Combine(RootPath, _configuration[path]!);
        string pathUrl = Path.Combine(RootURL, _configuration[path]!);
        var res = await FileHelper.UploadFile(L, file, pathBase, new List<string>() { ".jpg", ".jpeg", ".png" }, pathUrl);

        return new BaseMedia
        {
            Extension = res.Extension,
            FileName = res.Name,
            FileNameUpload = res.NameUpload,
            FilePath = res.Url.Replace(RootURL, ""),
            Type = "Image"
        };
    }
}

