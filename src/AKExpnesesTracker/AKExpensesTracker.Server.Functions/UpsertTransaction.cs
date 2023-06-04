using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data.Interfaces;
using AKExpensesTracker.Server.Data.Models;
using AKExpensesTracker.Shared.DTOs;
using AKExpensesTracker.Shared.Responses;
using FluentValidation;
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
    public class UpsertTransaction
    {
        private readonly ILogger<UpsertTransaction> _logger;
        private readonly ITransactionsRepository _transactionsRepo;
		private readonly IWalletsRepository _walletsRepo;
		private readonly IAttachmentsRepository _attachmentsRepo;
		private readonly IValidator<TransactionDto> _validator;
		public UpsertTransaction(ILogger<UpsertTransaction> log,
								 ITransactionsRepository transactionsRepo,
								 IWalletsRepository walletsRepo,
								 IAttachmentsRepository attachmentsRepo,
								 IValidator<TransactionDto> validator)
		{
			_logger = log;
			_transactionsRepo = transactionsRepo;
			_walletsRepo = walletsRepo;
			_attachmentsRepo = attachmentsRepo;
			_validator = validator;
		}

		[FunctionName("UpsertTransaction")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");
			_logger.LogInformation("Upsert wallet started");
			var userId = "userId"; // TODO: Fetch it from the access token 

			var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			var data = JsonConvert.DeserializeObject<TransactionDto>(requestBody);

			// Check if it's update or insert 
			bool isUpdate = data.Id != null;

			if (isUpdate)
			{

			}
			else
			{
				var validationResult = _validator.Validate(data);
				if (!validationResult.IsValid)
				{
					return new BadRequestObjectResult(new ApiErrorResponse("Invalid input", validationResult.Errors.Select(x => x.ErrorMessage)));
				}

				// Check if there is attachments 
				IEnumerable<Attachment> attachments = null;
				if (data.Attachments != null && data.Attachments.Length > 0)
				{
					attachments = await _attachmentsRepo.GetByURLsAsync(data.Attachments);
					if (attachments.Count() != data.Attachments.Distinct().Count())
						return new BadRequestObjectResult(new ApiErrorResponse("Attachment is wrong"));
				}

				var wallet = await _walletsRepo.GetByIdAsync(data.WalletId, userId);
				if (wallet == null)
					return new BadRequestObjectResult(new ApiErrorResponse("Wallet not found"));

				// Check the amount 
				var amountToAdd = data.IsIncome ? data.Amount : -data.Amount;

				// Create the transaction
				var transaction = Transaction.Create(userId,
													 data.Amount,
													 wallet.Id,
													 data.Category,
													 data.IsIncome,
													 data.Description,
													 data.Tags,
													 attachments?.Select(a => a.Url).ToArray());

				// Add the transaction to the database
				await _transactionsRepo.CreateAsync(transaction);

				// Update the wallet balance
				wallet.Balance += amountToAdd;
				await _walletsRepo.UpdateAsync(wallet);

				data.Id = transaction.Id;
				return new OkObjectResult(new ApiSuccessResponse<TransactionDto>("Transaction added successfully", data));
			}


			return new OkResult();
		}

		private async Task AddRandomTransactions(string userId)
		{
			var rand = new Random();

			// Available categories and tags to choose from
			var categories = new[] { "Food", "Beverage", "Clothing", "Grocery" };
			var foodTags = new[] { "Pizza", "Junk Food", "Chinese Cuisine", "Fast Food" };
			var beverageTags = new[] { "Water", "Soda", "Tea", "Coffee" };
			var clothingTags = new[] { "Shirt", "Pants", "Socks", "Jacket" };
			var groceryTags = new[] { "Fruit", "Vegetables", "Dairy", "Bread" };

			for (int i = 0; i < 20; i++)
			{
				var category = categories[rand.Next(categories.Length)];
				string[] tags;

				switch (category)
				{
					case "Food":
						tags = foodTags;
						break;
					case "Beverage":
						tags = beverageTags;
						break;
					case "Clothing":
						tags = clothingTags;
						break;
					case "Grocery":
						tags = groceryTags;
						break;
					default:
						tags = new[] { "Tag 1", "Tag 2" };
						break;
				}

				var walletId = "1209973d-86da-4983-a128-7065e75a65a1";
				var wallet = await _walletsRepo.GetByIdAsync(walletId, userId);
				var transaction = Transaction.Create(userId, Convert.ToDecimal(rand.Next(1, 100)), walletId, category, false, tags: tags);
				await _transactionsRepo.CreateAsync(transaction);
				var amountToAdd = transaction.IsIncome ? transaction.Amount : -transaction.Amount;
				//await _walletsRepo.UpdateWalletBalanceAsync(walletId, userId,  wallet.Balance + amountToAdd);
				wallet.Balance += amountToAdd;
				await _walletsRepo.UpdateAsync(wallet);
			}
		}
	}
}

