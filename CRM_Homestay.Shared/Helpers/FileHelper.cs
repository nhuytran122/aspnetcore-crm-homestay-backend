using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Localization;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Net;

namespace CRM_Homestay.Core.Helpers;

public class FileHelper
{
    public static async Task<FileModel> UploadFile(ILocalizer L, IFormFile file, string basePath, List<string> ExtensionsConstraint, string baseUri = "")
    {
        string directoryPath = "";
        var filePath = "";
        var fileDto = new FileModel();
        if (file.Length > 0)
        {
            var ext = Path.GetExtension(file.FileName);


            if (!ExtensionsConstraint.Any(x => x.Contains(ext)))
            {
                throw new GlobalException(L[UploadErrorCode.InvalidExtension], HttpStatusCode.BadRequest);
            }

            var directory = $"{DateTime.UtcNow:dd-MM-yyyy}";
            directoryPath = Path.GetFullPath(Path.Combine(basePath, directory));
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var StorageFileName = $"{Guid.NewGuid()}{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}{Guid.NewGuid()}{file.FileName}";
            filePath = Path.Combine(directoryPath, StorageFileName);
            await CompressAndSaveImageAsync(file, filePath, 50);
            fileDto.Extension = ext;
            fileDto.Path = filePath;
            fileDto.Name = StorageFileName;
            fileDto.NameUpload = file.FileName;
            if (baseUri != "")
            {
                fileDto.Url = Path.Combine(baseUri, directory, StorageFileName);
            }
            return fileDto;
        }
        else
        {
            return fileDto;
        }
    }

    private static async Task CompressAndSaveImageAsync(IFormFile imageFile, string outputPath, int quality = 60)
    {
        using var imageStream = imageFile.OpenReadStream();
        using var image = Image.Load(imageStream);
        var encoder = new JpegEncoder
        {
            Quality = quality, // Adjust this value for desired compression quality
        };

        await Task.Run(() => image.Save(outputPath, encoder));
    }

    public class FileModel
    {
        public string Path { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NameUpload { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}