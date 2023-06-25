using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data.Interfaces;
using AKExpensesTracker.Server.Functions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

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

		[FunctionName("DeleteTransaction")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var userId = "userId"; // TODO: Fetch it from the access token
            string id = req.Query["id"];

            if (string.IsNullOrWhiteSpace(id))
            {
				return new BadRequestObjectResult("Id is required");
			}

            var year = req.Query["year"];
            // validate the year 
            if (int.TryParse(year, out int yearInt) == false)
            {
				return new BadRequestObjectResult("Year is required");
			}

            var transaction = await _transactionsRepo.GetByIdAsync(id, userId, yearInt);
            if (transaction == null)
            {
                return new NotFoundResult(); 
            }

            // Delete the attachments
            if (transaction.Attachments != null && transaction.Attachments.Any())
            {
				foreach (var attachment in transaction.Attachments)
                {
					await _storageService.DeleteFileAsync(attachment);
				}
			}

            // Delete the transaction
            await _transactionsRepo.DeleteAsync(transaction);

            return new OkResult();
        }
    }
}

