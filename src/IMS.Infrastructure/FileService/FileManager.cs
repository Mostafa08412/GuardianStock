
using IMS.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace IMS.Infrastructure.FileService;

public class FileManager : IFileManager
{
    private readonly IWebHostEnvironment hostEnvironment;

    public FileManager(IWebHostEnvironment hostEnvironment)
    {
        this.hostEnvironment = hostEnvironment;
    }

    public void DeleteFileIfExists(string fileName, string directory, string extension)
    {
        var formattedExtension = extension.StartsWith(".") ? extension : $".{extension}";

        var fileNameWithExt = Path.HasExtension(fileName)
            ? fileName
            : Path.ChangeExtension(fileName, formattedExtension);

        var fileDirectory = Path.Combine(hostEnvironment.ContentRootPath, directory);
        var fileFullPath = Path.Combine(fileDirectory, fileNameWithExt);

        if (File.Exists(fileFullPath))
        {
            File.Delete(fileFullPath);
        }
    }


    public async Task<(string, long)> GetFileExtensionAndSizeInMbAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var fileExtension = GetExtensionIfValid(file.FileName);

        var fileSizeinMb = (long)(await ConvertFileToBytesAsync(file, cancellationToken)).Length / (1024 * 1024);

        return (fileExtension, fileSizeinMb);
    }

    public async Task<string> UploadFileAsync(IFormFile file, string directory, string fileName, CancellationToken cancellationToken = default)
    {
        var fileExtension = GetExtensionIfValid(file.FileName);

        var path = Path.Combine(hostEnvironment.WebRootPath, directory);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var filePath = Path.Combine(path, $"{fileName}.{fileExtension}");

        await File.WriteAllBytesAsync(filePath, await ConvertFileToBytesAsync(file, cancellationToken), cancellationToken);

        return filePath;
    }

    private string GetExtensionIfValid(string fileName)
    {
        //return csv
        //dot in not included in the return.
        var fileExtension = Path.GetExtension(fileName).TrimStart('.').ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(fileExtension))
        {
            throw new ArgumentException("File extension is invalid.", fileName);
        }

        return fileExtension;
    }

    private async Task<byte[]> ConvertFileToBytesAsync(IFormFile file, CancellationToken cancellationToken)
    {

        if (file == null)
        {
            throw new ArgumentNullException(nameof(file), "File is null.");
        }



        using var ms = new MemoryStream();

        await file.CopyToAsync(ms, cancellationToken);

        var fileBytes = ms.ToArray();

        if (fileBytes.Length == 0)
        {
            throw new ArgumentException("File is empty.", nameof(file));
        }


        return fileBytes;
    }

}
