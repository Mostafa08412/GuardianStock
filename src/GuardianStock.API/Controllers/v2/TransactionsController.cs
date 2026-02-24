using Asp.Versioning;
using GuardianStock.Application.Common.Models;
using GuardianStock.Application.Transactions.Commands.RecordPurchase;
using GuardianStock.Application.Transactions.Commands.RecordSale;
using GuardianStock.Application.Transactions.Queries.GetTransaction;
using GuardianStock.Application.Transactions.Queries.ListTransactions;
using GuardianStock.API.Contracts;
using GuardianStock.API.Contracts.Examples;
using GuardianStock.API.Infrastructure;
using GuardianStock.Application.Transactions.Queries.GetTransaction;
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
    [Route(ApiRoutes.Transactions.Base)]
    [SwaggerResponse((int)ApplicationStatusCodes.Unauthorized, "The request is missing a valid authentication token.", typeof(ApiResponse))]
    [SwaggerResponse((int)ApplicationStatusCodes.Forbidden, "The authenticated user does not have the 'Admin' role required for this resource.", typeof(ApiResponse))]
    [SwaggerResponse((int)ApplicationStatusCodes.BadRequest, "The request payload is invalid or malformed.", typeof(ApiResponse))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.Unauthorized, typeof(ApiResponseExample))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.Forbidden, typeof(ApiResponseExample))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.BadRequest, typeof(ApiResponseExample))]

    public class TransactionsController : BaseController
    {
        public TransactionsController(ISender sender) : base(sender)
        {
        }

        /// <summary>
        /// Retrieves a paginated list of transactions filtered by various criteria.
        /// Supported sorting columns: date (default), amount, quantity.
        /// </summary>
        /// <param name="query">Filtering and pagination parameters.</param>
        /// <returns>A paginated list of transactions summary.</returns>
        [Authorize]
        [HttpGet(ApiRoutes.Transactions.GetAll)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<PaginatedList<TransactionDto>>))]
        [SwaggerOperation(
            Summary = "List transactions",
            Description = "Fetches a paginated list of inventory transactions with support for filtering by date, type, amount, and SKU." +
            "\nSupported sorting columns: date (default), amount, quantity.",
            OperationId = "ListTransactions"
        )]
        public async Task<IActionResult> ListTransactions([FromQuery] ListTransactionsQuery query)
        {
            var result = await _sender.Send(query);

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Retrieves detailed information for a specific transaction.
        /// </summary>
        /// <param name="transactionId">The unique identifier of the transaction.</param>
        /// <returns>Full details of the transaction.</returns>
        [Authorize]
        [HttpGet(ApiRoutes.Transactions.GetById)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<TransactionDetailsDto>))]
        [ProducesResponseType((int)ApplicationStatusCodes.NotFound, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Get transaction by ID",
            Description = "Retrieves full details for a single transaction using its unique identifier.",
            OperationId = "GetTransactionById"
        )]
        public async Task<IActionResult> GetById([FromRoute] Guid transactionId)
        {
            var result = await _sender.Send(new GetTransactionQuery(transactionId));

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Records a new sale transaction (decreases inventory).
        /// </summary>
        /// <param name="command">The sale transaction details.</param>
        /// <returns>No content on success.</returns>
        [Authorize]
        [HttpPost(ApiRoutes.Transactions.RecordSale)]
        [ProducesResponseType((int)ApplicationStatusCodes.NoContent, Type = typeof(ApiResponse))]
        [ProducesResponseType((int)ApplicationStatusCodes.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Record sale",
            Description = "Creates a new sale transaction and updates the corresponding product's inventory level. Fails if insufficient stock.",
            OperationId = "RecordSale"
        )]
        public async Task<IActionResult> RecordSale([FromBody] RecordSaleCommand command)
        {
            var result = await _sender.Send(command);

            return HandleResult(result, ApplicationStatusCodes.NoContent);
        }

        /// <summary>
        /// Records a new purchase transaction (increases inventory).
        /// </summary>
        /// <param name="command">The purchase transaction details.</param>
        /// <returns>No content on success.</returns>
        [Authorize]
        [HttpPost(ApiRoutes.Transactions.RecordPurchase)]
        [ProducesResponseType((int)ApplicationStatusCodes.NoContent, Type = typeof(ApiResponse))]
        [ProducesResponseType((int)ApplicationStatusCodes.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Record purchase",
            Description = "Creates a new purchase transaction and increases the corresponding product's inventory level.",
            OperationId = "RecordPurchase"
        )]
        public async Task<IActionResult> RecordPurchase([FromBody] RecordPurchaseCommand command)
        {
            var result = await _sender.Send(command);

            return HandleResult(result, ApplicationStatusCodes.NoContent);
        }
    }
}
