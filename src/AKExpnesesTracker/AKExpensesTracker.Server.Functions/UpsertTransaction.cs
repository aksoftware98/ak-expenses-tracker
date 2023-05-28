using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data.Interfaces;
using AKExpensesTracker.Server.Data.Models;
using AKExpensesTracker.Shared.DTOs;
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
		public UpsertTransaction(ILogger<UpsertTransaction> log,
								 ITransactionsRepository transactionsRepo,
								 IWalletsRepository walletsRepo)
		{
			_logger = log;
			_transactionsRepo = transactionsRepo;
			_walletsRepo = walletsRepo;
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

			// Read the string from the body (JSON)
			var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			// Deserialize the string to WalletDto object 
			var data = JsonConvert.DeserializeObject<WalletDto>(requestBody);

			// Random number generator
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

	
			return new OkResult();
        }
    }
}

