using Asp.Versioning;
using GuardianStock.Application.Common.Models;
using GuardianStock.Application.Products.Commands.ConfirmImport;
using GuardianStock.Application.Products.Commands.CreateProduct;
using GuardianStock.Application.Products.Commands.UpdateProduct;
using GuardianStock.Application.Products.Commands.UploadProductCsv;
using GuardianStock.Application.Products.Queries.GetProductDetails;
using GuardianStock.Application.Products.Queries.ListProducts;
using GuardianStock.API.Contracts;
using GuardianStock.API.Contracts.Examples;
using GuardianStock.API.Infrastructure;
using GuardianStock.Application.Products.Commands.DeleteProduct;
using GuardianStock.Application.Products.Queries.GetProductDetails;
using GuardianStock.Application.Products.Queries.GetProductImage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Roles = GuardianStock.Domain.Enums.Roles;

namespace GuardianStock.API.Controllers.v2
{
    [ApiController]
    [ApiVersion(2.0)]
    [Route(ApiRoutes.Products.Base)]
    [SwaggerResponse((int)ApplicationStatusCodes.Unauthorized, "The request is missing a valid authentication token.", typeof(ApiResponse))]
    [SwaggerResponse((int)ApplicationStatusCodes.Forbidden, "The authenticated user does not have the 'Admin' role required for this resource.", typeof(ApiResponse))]
    [SwaggerResponse((int)ApplicationStatusCodes.BadRequest, "The request payload is invalid or malformed.", typeof(ApiResponse))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.Unauthorized, typeof(ApiResponseExample))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.Forbidden, typeof(ApiResponseExample))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.BadRequest, typeof(ApiResponseExample))]

    public class ProductsController : BaseController
    {
        public ProductsController(ISender sender) : base(sender)
        {
        }

        /// <summary>
        /// Retrieves a paginated list of products with filtering and sorting support.
        /// Supported sorting columns: sku, name, price, stock (default: name).
        /// </summary>
        /// <param name="query">Filtering and pagination parameters.</param>
        /// <returns>A paginated list of product summaries.</returns>
        [Authorize]
        [HttpGet(ApiRoutes.Products.GetAll)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<PaginatedList<ProductListItemDto>>))]
        [SwaggerOperation(
            Summary = "List products",
            Description = "Fetches a paginated list of products with support for searching (name, SKU, description, supplier) and " +
            "filtering by category, stock level (Normal, Low, Critical), and price range." +
            "\nSupported sorting columns: sku, name, price, stock (default: name).",
            OperationId = "ListProducts"
        )]
        public async Task<IActionResult> GetAll([FromQuery] ListProductsQuery query)
        {
            var result = await _sender.Send(query);

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Retrieves detailed information for a specific product.
        /// </summary>
        /// <param name="productId">The unique identifier of the product.</param>
        /// <returns>The product details.</returns>
        [Authorize]
        [HttpGet(ApiRoutes.Products.GetById)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<ProductDetailsDto>))]
        [ProducesResponseType((int)ApplicationStatusCodes.NotFound, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Get product by ID",
            Description = "Retrieves full details for a single product using its unique identifier.",
            OperationId = "GetProductById"
        )]
        public async Task<IActionResult> GetById([FromRoute] Guid productId)
        {
            var result = await _sender.Send(new GetProductDetailsQuery(productId));

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }



        /// <summary>
        /// Creates a new product in the system.
        /// </summary>
        /// <param name="command">The details of the product to create.</param>
        /// <returns>The unique identifier of the newly created product.</returns>
        [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
        [HttpPost(ApiRoutes.Products.Create)]
        [ProducesResponseType((int)ApplicationStatusCodes.Created, Type = typeof(ApiResponse<Guid>))]
        [ProducesResponseType((int)ApplicationStatusCodes.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Create product",
            Description = "Creates a new product with an initial quantity and low stock threshold.",
            OperationId = "CreateProduct"
        )]
        public async Task<IActionResult> Create([FromForm] CreateProductCommand command)
        {
            var result = await _sender.Send(command);

            return HandleResult(result, ApplicationStatusCodes.Created);
        }

        /// <summary>
        /// Updates an existing product's information.
        /// </summary>
        /// <param name="productId">The unique identifier of the product to update.</param>
        /// <param name="command">The updated product details.</param>
        /// <returns>No content on success.</returns>
        [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
        [HttpPut(ApiRoutes.Products.Update)]
        [ProducesResponseType((int)ApplicationStatusCodes.NoContent, Type = typeof(ApiResponse))]
        [ProducesResponseType((int)ApplicationStatusCodes.BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType((int)ApplicationStatusCodes.NotFound, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Update product",
            Description = "Updates the metadata and settings for an existing product. Ensures the route ID matches the body ID.",
            OperationId = "UpdateProduct"
        )]
        public async Task<IActionResult> Update([FromRoute] Guid productId, [FromForm] UpdateProductCommand command)
        {
            if (productId != command.ProductId)
            {
                return BadRequest("The product ID in the route does not match the product ID in the request body.");
            }

            var result = await _sender.Send(command);

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Deletes a product from the system.
        /// </summary>
        /// <param name="productId">The unique identifier of the product to delete.</param>
        /// <returns>No content on success.</returns>
        [Authorize(Roles = $"{Roles.Admin}")]
        [HttpDelete(ApiRoutes.Products.Delete)]
        [ProducesResponseType((int)ApplicationStatusCodes.NoContent, Type = typeof(ApiResponse))]
        [ProducesResponseType((int)ApplicationStatusCodes.NotFound, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Delete product",
            Description = "Removes a product from the inventory system.",
            OperationId = "DeleteProduct"
        )]
        public async Task<IActionResult> Delete([FromRoute] Guid productId)
        {
            var result = await _sender.Send(new DeleteProductCommand(productId));

            return HandleResult(result, ApplicationStatusCodes.NoContent);
        }




        /// <summary>
        /// Uploads a product CSV file for validation and preview generation.
        /// </summary>
        /// <param name="request">The CSV upload request containing the file.</param>
        /// <returns>A job identifier that can be used to track the preview generation.</returns>
        [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
        [HttpPost(ApiRoutes.Products.Upload)]
        [ProducesResponseType((int)ApplicationStatusCodes.Accepted, Type = typeof(ApiResponse<UploadProductCsvResponse>))]
        [SwaggerOperation(
            Summary = "Upload product CSV",
            Description = "Uploads a product CSV file for validation and preview generation. Returns a job identifier to track progress.",
            OperationId = "UploadProductCsv"
        )]
        public async Task<IActionResult> UploadCSV([FromForm] UploadProductCsvCommand request)
        {
            var result = await _sender.Send(request);

            return HandleResult(result, ApplicationStatusCodes.Accepted);
        }


        /// <summary>
        /// Confirms and executes the import of a previously uploaded product CSV file.
        /// </summary>
        /// <param name="command">The confirm import command containing the job identifier.</param>
        /// <returns>Accepted if the import has been queued successfully.</returns>
        [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
        [HttpPost(ApiRoutes.Products.ConfirmImport)]
        [ProducesResponseType((int)ApplicationStatusCodes.Accepted, Type = typeof(ApiResponse<Unit>))]
        [SwaggerOperation(
            Summary = "Confirm product CSV import",
            Description = "Confirms and executes the import of a previously uploaded and validated product CSV file.",
            OperationId = "ConfirmProductCsvImport"
        )]
        public async Task<IActionResult> Import([FromForm] ConfirmImportCommand command)
        {
            var result = await _sender.Send(command);

            return HandleResult(result, ApplicationStatusCodes.Accepted);
        }

        /// <summary>
        /// Retrieves a product image by its filename.
        /// </summary>
        /// <param name="imageName">The filename of the product image.</param>
        /// <returns>The product image file.</returns>
        [HttpGet("images/{imageName}")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Get product image",
            Description = "Retrieves a product image by its filename.",
            OperationId = "GetProductImage"
        )]
        public async Task<IActionResult> GetImage([FromRoute] string imageName)
        {
            var result = await _sender.Send(new GetProductImageQuery(imageName));

            if (result.IsFailure)
                return HandleResult(result, ApplicationStatusCodes.Ok);

            return PhysicalFile(result.Value!.FilePath, result.Value!.ContentType);
        }

    }
}
