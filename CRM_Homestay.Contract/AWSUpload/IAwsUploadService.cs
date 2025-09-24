using CRM_Homestay.Contract.Medias;
using Microsoft.AspNetCore.Http;

namespace CRM_Homestay.Contract.AWSUpload
{
    public interface IAwsUploadService
    {
        Task<MediaDto> UploadAvatar(IFormFile file);
        Task<List<string>> UploadImgs(List<IFormFile> files, string path);
        string GetRootUrl();
        Task<bool> DeleteImage(string imageUrl);
        Task<string> UploadImg(IFormFile file, string path);
    }
}
