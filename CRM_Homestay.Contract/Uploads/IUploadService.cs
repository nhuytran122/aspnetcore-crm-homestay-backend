
using Microsoft.AspNetCore.Http;
using CRM_Homestay.Contract.Medias;
using CRM_Homestay.Entity.Medias;

namespace CRM_Homestay.Contract.Uploads;

public interface IUploadService
{
    string GetRootUrl();
    string GetRootPath();
    Task<MediaDto> UploadAvatar(IFormFile file);
    Task<string> UploadProduct(IFormFile file);
    Task<List<string>> UploadPaymentImg(List<IFormFile> file);
    public bool DeleteImage(string imageUrl);
    Task<List<BaseMedia>> UploadImgs(List<IFormFile> file, string path);
    Task<BaseMedia> UploadImg(IFormFile file, string path);
    bool DeleteImages(List<string> imageUrls);
}