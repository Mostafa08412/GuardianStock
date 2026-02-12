using MediatR;

namespace IMS.Application.Products.Commands.GeneratePreview
{
    public record GeneratePreviewCommand : IRequest
    {
        public string filePath { get; init; }

        public string userId { get; init; }
        public string jobId { get; init; }
    }
}
