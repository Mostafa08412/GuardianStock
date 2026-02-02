using IMS.Application.Products.Commands.ImportProducts.Dtos;
using IMS.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace IMS.Application.Products.Commands.ImportProducts
{
    public record ImportProductsCommand : IRequest<Result<ImportProductsReport>>
    {
        public IFormFile? file { get; init; }
    }
}
