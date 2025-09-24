using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using AutoMapper;
using CRM_Homestay.Contract.AWSUpload;
using CRM_Homestay.Contract.Medias;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Net;
using static CRM_Homestay.Core.Helpers.FileHelper;

namespace CRM_Homestay.Service.AWSUpload
{
    public class AwsUploadService : BaseService, IAwsUploadService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;
        private string BucketName { get; set; }
        private string RootURL { get; set; }

        public AwsUploadService(IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l, IAmazonS3 s3Client, IConfiguration configuration) : base(unitOfWork, mapper, l)
        {
            _s3Client = s3Client;
            _configuration = configuration;
            BucketName = _configuration["BucketName"]!;
            RootURL = $"https://{BucketName}.s3.amazonaws.com";
        }
        public string GetRootUrl()
        {
            return RootURL;
        }

        public async Task<MediaDto> UploadAvatar(IFormFile file)
        {
            string folder = _configuration["Media:Avatars"]!;
            var res = await S3UploadCompressedFile(L, file, folder, new List<string>() { ".jpg", ".jpeg", ".png" }, BucketName);
            return new MediaDto() { Url = res.Url, Extension = res.Extension, Name = res.Name };
        }

        public async Task<string> UploadImg(IFormFile file, string path)
        {
            string folder = _configuration[path]!;
            //string folder = "hueTest";
            return await UploadBaseImg(file, folder);
        }
        public async Task<List<string>> UploadImgs(List<IFormFile> files, string path)
        {
            string folder = _configuration[path]!;
            var urlsList = new List<string>();
            long maxFileSize = 100 * 1024 * 1024; // 100MB
            var invalidFiles = files.Where(file => file.Length > maxFileSize).ToList();
            if (invalidFiles.Any())
            {
                throw new GlobalException(L[UploadErrorCode.InvalidLength], HttpStatusCode.BadRequest);
            }
            foreach (var item in files)
            {
                urlsList.Add(await UploadBaseImg(item, folder));
            }
            return urlsList;
        }

        public async Task<string> UploadBaseImg(IFormFile file, string folder)
        {
            var res = await S3UploadCompressedFile(L, file, folder, new List<string>() { ".jpg", ".jpeg", ".png", ".mp4", ".avi", ".mov", ".mkv", ".wmv", ".flv", ".mpeg" }, BucketName);
            return res.Url!.Replace(RootURL, "");
        }

        public async Task<bool> DeleteImage(string imageUrl)
        {
            try
            {
                var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, BucketName);
                if (!bucketExists)
                {
                    throw new GlobalException(L[BaseErrorCode.BucketNotExist, BucketName], HttpStatusCode.NotFound);
                }
                await _s3Client.DeleteObjectAsync(BucketName, imageUrl.TrimStart('/'));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<FileModel> S3UploadFile(ILocalizer L, IFormFile file, string basePath, List<string> ExtensionsConstraint, string bucketName)
        {
            var fileDto = new FileModel();
            if (file.Length > 0)
            {
                var ext = Path.GetExtension(file.FileName);


                if (!ExtensionsConstraint.Any(x => x.Contains(ext)))
                {
                    throw new GlobalException(L[UploadErrorCode.InvalidExtension], HttpStatusCode.BadRequest);
                }

                var directory = $"{basePath}/{DateTime.UtcNow:dd-MM-yyyy}";
                var fileName = $"{Guid.NewGuid()}{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}{Guid.NewGuid()}{file.FileName}";

                var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
                if (!bucketExists)
                {
                    throw new GlobalException(L[BaseErrorCode.BucketNotExist, bucketName], HttpStatusCode.NotFound);
                }

                var key = $"{directory}/{fileName}";

                var request = new PutObjectRequest()
                {
                    BucketName = bucketName,
                    Key = key,
                    InputStream = file.OpenReadStream(),
                    CannedACL = S3CannedACL.PublicRead // Thêm quyền truy cập public
                };

                request.Metadata.Add("Content-Type", file.ContentType);

                await _s3Client.PutObjectAsync(request);

                // Lấy URL public của đối tượng vừa tạo
                var publicUrl = $"https://{bucketName}.s3.amazonaws.com/{key}";

                fileDto.Extension = ext;
                fileDto.Name = file.FileName;
                fileDto.Url = publicUrl;
                return fileDto;
            }
            else
            {
                return fileDto;
            }

        }

        private async Task<FileModel> S3UploadCompressedFile(ILocalizer L, IFormFile file, string basePath, List<string> ExtensionsConstraint, string bucketName)
        {
            var fileDto = new FileModel();
            if (file.Length > 0)
            {
                var ext = Path.GetExtension(file.FileName);

                if (!ExtensionsConstraint.Any(x => x.Contains(ext)))
                {
                    throw new GlobalException(L[UploadErrorCode.InvalidExtension], HttpStatusCode.BadRequest);
                }

                var directory = $"{basePath}/{DateTime.UtcNow:dd-MM-yyyy}";
                var fileName = $"{Guid.NewGuid()}{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}{Guid.NewGuid()}{file.FileName}";

                var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
                if (!bucketExists)
                {
                    throw new GlobalException(L[BaseErrorCode.BucketNotExist, bucketName], HttpStatusCode.NotFound);
                }

                var key = $"{directory}/{fileName}";
                PutObjectRequest putObject;
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                {
                    var compressedData = await CompressImageAsync(file);
                    putObject = new PutObjectRequest()
                    {
                        BucketName = bucketName,
                        Key = key,
                        InputStream = new MemoryStream(compressedData),
                        CannedACL = S3CannedACL.PublicRead
                    };
                }
                else
                {
                    putObject = new PutObjectRequest()
                    {
                        BucketName = bucketName,
                        Key = key,
                        InputStream = file.OpenReadStream(),
                        CannedACL = S3CannedACL.PublicRead
                    };
                }
                putObject.Metadata.Add("Content-Type", file.ContentType);
                await _s3Client.PutObjectAsync(putObject);
                var publicUrl = $"https://{bucketName}.s3.amazonaws.com/{key}";
                fileDto.Extension = ext;
                fileDto.Name = file.FileName;
                fileDto.Url = publicUrl;
            }
            return fileDto;
        }

        private async Task<byte[]> CompressImageAsync(IFormFile imageFile, int quality = 50)
        {
            using var imageStream = imageFile.OpenReadStream();
            using var image = Image.Load(imageStream);
            image.Mutate(x =>
            {
                if (image.Width > 800)
                {
                    x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(800) // New width of 800px, height is calculated to maintain aspect ratio
                    });
                }
            });
            var encoder = new JpegEncoder
            {
                Quality = quality
            };
            using var outputStream = new MemoryStream();
            await image.SaveAsync(outputStream, encoder);
            return outputStream.ToArray();
        }
    }
}
