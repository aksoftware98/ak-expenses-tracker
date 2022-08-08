using System.IO;
using System.Net;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data.Interfaces;
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
    public class GetWalletDetails
    {
        private readonly ILogger<GetWalletDetails> _logger;
        private readonly IWalletsRepository _walletsRepo;

        public GetWalletDetails(ILogger<GetWalletDetails> log, 
                                IWalletsRepository walletsRepo)
        {
            _logger = log;
            _walletsRepo = walletsRepo;
        }

        [FunctionName("GetWalletDetails")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            // TODO: Fetch the user id from the access token
            var userId = "userId";
            var walletId = req.Query["id"];
            
            _logger.LogInformation($"Retrieve the wallet with id {walletId} for the user {userId}");
            if (string.IsNullOrWhiteSpace(walletId))
            {
                return new BadRequestObjectResult(new ApiErrorResponse("Wallet Id is required"));
            }

            var wallet = await _walletsRepo.GetByIdAsync(walletId, userId);
            if (wallet == null)
                return new NotFoundResult(); // Retrieve 404 response code 
            else
            {
                return new OkObjectResult(new WalletDto
                {
                    Id = wallet.Id,
                    Name = wallet.Name,
                    AccountType = wallet.AccountType,
                    Balance = wallet.Balance,
                    Currency = wallet.Currency,
                    BankName = wallet.BankName,
                    CreationDate = wallet.CreationDate,
                    Iban = wallet.Iban,
                    Swift = wallet.Swift,
                    Type = wallet.Type.Value,
                    Username = wallet.Username,
                });  // Retrieve 200 with respose body
            } 

        }
    }
}

