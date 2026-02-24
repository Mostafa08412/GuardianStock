using Microsoft.AspNetCore.Http;

namespace GuardianStock.Application.Common.Interfaces
{
    public interface IFileManagerService
    {
        void DeleteFileIfExists(string fileName, string directory, string extension);
        Task<(string, long)> GetFileExtensionAndSizeInMbAsync(IFormFile file, CancellationToken cancellationToken);
        Task<string> UploadFileAsync(IFormFile file, string directory, string fileName, CancellationToken cancellationToken = default);
        string? GetFilePath(string fileName, string directory);
    }
}
