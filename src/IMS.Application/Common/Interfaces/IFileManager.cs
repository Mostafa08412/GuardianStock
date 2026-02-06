using Microsoft.AspNetCore.Http;

namespace IMS.Application.Common.Interfaces
{
    public interface IFileManager
    {
        void DeleteFileIfExists(string fileName, string directory, string extension);
        Task<(string, long)> GetFileExtensionAndSizeInMbAsync(IFormFile file, CancellationToken cancellationToken);
        Task<string> UploadFileAsync(IFormFile file, string directory, string fileName, CancellationToken cancellationToken = default);
    }
}
