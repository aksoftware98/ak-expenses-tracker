using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data.Interfaces;
using AKExpensesTracker.Server.Functions.Mapper;
using AKExpensesTracker.Shared.DTOs;
using AKExpensesTracker.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace AKExpensesTracker.Server.Functions
{
    public class ListTransactions
    {
        private readonly ILogger<ListTransactions> _logger;
        private readonly ITransactionsRepository _transactionsRepo;
		public ListTransactions(ILogger<ListTransactions> log, ITransactionsRepository transactionsRepo)
		{
			_logger = log;
			_transactionsRepo = transactionsRepo;
		}

		[FunctionName("ListTransactions")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("List transactions");
            var userId = "userId"; // TODO: Fetch from the access token

            string yearParameter = req.Query["year"];
            string minDateParameter = req.Query["minDate"];
            string maxDateParameter = req.Query["maxDate"];
            var walletIds = req.Query["walletIds"];

            if (string.IsNullOrWhiteSpace(yearParameter))
				return new BadRequestObjectResult(new ApiErrorResponse("Year is required"));
            if (!walletIds.Any())
				return new BadRequestObjectResult(new ApiErrorResponse("At least one wallet is required"));

            int year = 2023;
            DateTime? minDate = null;
            DateTime? maxDate = null;
            if (!int.TryParse(yearParameter, out year))
                return new BadRequestObjectResult(new ApiErrorResponse("Year is invalid"));

            if (!string.IsNullOrWhiteSpace(minDateParameter) && DateTime.TryParse(minDateParameter, out var minDateValue))
                minDate = minDateValue;

			if (!string.IsNullOrWhiteSpace(maxDateParameter) && DateTime.TryParse(maxDateParameter, out var maxDateValue))
				maxDate = maxDateValue;

            var transactions = await _transactionsRepo.ListByUserIdAsync(userId, year, walletIds, minDate, maxDate);

			return new OkObjectResult(new ApiSuccessResponse<IEnumerable<TransactionDto>>("Transactions retreived successfully", 
                                                                                           transactions.Select(t => t.ToTransactionDto())));
        }
    }
}

