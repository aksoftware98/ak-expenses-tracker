using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data.Interfaces;
using AKExpensesTracker.Server.Data.Models;
using AKExpensesTracker.Shared.DTOs;
using AKExpensesTracker.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;

namespace AKEpensesTracker.Server.Functions
{
    public class ListWallets
    {
        private readonly ILogger<ListWallets> _logger;
        private readonly IWalletsRepository _walletsRepo;
        public ListWallets(ILogger<ListWallets> log,
                           IWalletsRepository walletsRepo)
        {
            _logger = log;
            _walletsRepo = walletsRepo;
        }

        [Function("ListWallets")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            var userId = "userId";

            var wallets = await _walletsRepo.ListByUserIdAsync(userId);
            var result = wallets.Select(w => new WalletSummaryDto
            {
                Id = w.Id,
                Name = w.Name,
                Currency = w.Currency,
                Balacne = w.Balance,
                Type = w.Type.Value
            });
            return new OkObjectResult(new ApiSuccessResponse<IEnumerable<WalletSummaryDto>>($"{wallets.Count()} have been retrieved", result)); // 200 
        }
    }
}

