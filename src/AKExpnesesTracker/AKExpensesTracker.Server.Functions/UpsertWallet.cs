using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data.Interfaces;
using AKExpensesTracker.Server.Data.Models;
using AKExpensesTracker.Shared.DTOs;
using AKExpensesTracker.Shared.Enums;
using AKExpensesTracker.Shared.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace AKExpensesTracker.Server.Functions
{
    public class UpsertWallet
    {
        private readonly ILogger<UpsertWallet> _logger;
        private readonly IWalletsRepository _walletsRepo;
        private readonly IValidator<WalletDto> _walletValidator;

        public UpsertWallet(ILogger<UpsertWallet> log,
                            IWalletsRepository walletsRepo,
                            IValidator<WalletDto> walletValidator)
        {
            _logger = log;
            _walletsRepo = walletsRepo;
            _walletValidator = walletValidator;
        }

        [FunctionName("UpsertWallet")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("Upsert wallet started");
            var userId = "userId"; // TODO: Fetch it from the access token 

            // Read the string from the body (JSON)
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // Deserialize the string to WalletDto object 
            var data = JsonConvert.DeserializeObject<WalletDto>(requestBody);

            // Input validation
            var validationResult = _walletValidator.Validate(data);
            if (!validationResult.IsValid)
            {
                _logger.LogError("ERROR - Upsert wallet Invalid input");
                return new BadRequestObjectResult(new ApiErrorResponse("Wallet inputs are not valid",
                                                                       validationResult.Errors.Select(e => e.ErrorMessage)));
            }
            // TODO: Validate the number of wallets based on the subscription type
            // Preperatoin phase 
            var isAdd = string.IsNullOrWhiteSpace(data.Id);
            Wallet wallet = null;
            // Add operation 
            if (isAdd)
            {
                wallet = new()
                {
                    CreationDate = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString()
                };

            }
            else // Update operation 
            {
                wallet = await _walletsRepo.GetByIdAsync(data.Id, userId);
                if (wallet == null)
                {
                    _logger.LogError("ERROR - Wallet not found");
                    return new NotFoundObjectResult(new ApiErrorResponse("Wallet not found"));
                }
            }

            wallet.Name = data.Name;
            wallet.AccountType = data.AccountType;
            wallet.Swift = data.Swift;
            wallet.BankName = data.BankName;
            wallet.Currency = data.Currency;
            wallet.Iban = data.Iban;
            wallet.ModificationDate = DateTime.UtcNow;
            wallet.UserId = userId;
            wallet.Username = data.Username;
            wallet.WalletType = data.Type.ToString();
            wallet.Balance = data.Balance;
            if (isAdd)
                await _walletsRepo.CreateAsync(wallet);
            else
                await _walletsRepo.UpdateAsync(wallet);

            // Set the auto genreated properties 
            data.Id = wallet.Id;
            data.CreationDate = wallet.CreationDate;

            return new OkObjectResult(new ApiSuccessResponse<WalletDto>($"Wallet {(isAdd ? "inserted" : "updated")} successfully", data));
        }
    }
}

