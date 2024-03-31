using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data.Interfaces;
using AKExpensesTracker.Server.Functions.Services;
using AKExpensesTracker.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;

namespace AKExpensesTracker.Server.Functions
{
    public class DeleteTransaction
    {
        private readonly ILogger<DeleteTransaction> _logger;
        private readonly ITransactionsRepository _transactionsRepo;
        private readonly IWalletsRepository _walletsRepo; 
        private readonly IStorageService _storageService;
		public DeleteTransaction(ILogger<DeleteTransaction> log, 
                                 ITransactionsRepository transactionsRepo, 
                                 IWalletsRepository walletsRepo, 
                                 IStorageService storageService)
		{
			_logger = log;
			_transactionsRepo = transactionsRepo;
			_walletsRepo = walletsRepo;
			_storageService = storageService;
		}

		[Function("DeleteTransaction")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("DeleteTransaction triggered.");
            var userId = "userId"; // TODO: Fetch from the token later
            var id = req.Query["id"];
            var year = req.Query["year"];
            if (string.IsNullOrWhiteSpace(id))
                return new BadRequestObjectResult(new ApiErrorResponse("Id is required"));
            if (string.IsNullOrWhiteSpace(year))
				return new BadRequestObjectResult(new ApiErrorResponse("Year is required"));

            if (!int.TryParse(year, out var yearAsInt))
            {
				return new BadRequestObjectResult(new ApiErrorResponse("Year is invalid"));
			}

            // Get transaction 
            var transaction = await _transactionsRepo.GetByIdAsync(id, userId, yearAsInt);

            if (transaction == null)
                return new NotFoundResult();

            // Delete attachments
            if (transaction.Attachments != null && transaction.Attachments.Any())
            {
                foreach (var url in transaction.Attachments)
                {
                    await _storageService.DeleteFileAsync(url);
                }
            }

            await _transactionsRepo.DeleteAsync(transaction);

            var amountToAdd = transaction.IsIncome ? -transaction.Amount : transaction.Amount;
            await _walletsRepo.UpdateBalanceAsync(transaction.WalletId, userId, amountToAdd);

			return new OkObjectResult(new ApiResponse()
            {
                Message = "Transaction has been deleted successfully"
            });
        }
    }
}

