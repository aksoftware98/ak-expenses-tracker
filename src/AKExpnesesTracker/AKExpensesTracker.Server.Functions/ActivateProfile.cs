using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AKExpensesTracker.Server.Functions
{
    public class ActivateProfile
    {
        private readonly ILogger<ActivateProfile> _logger;

        public ActivateProfile(ILogger<ActivateProfile> logger)
        {
            _logger = logger;
        }

        [Function("ActivateProfile")]
        public async Task<IActionResult> Run([HttpTrigger(Microsoft.Azure.Functions.Worker.AuthorizationLevel.Function, "post", Route = "activate/profile")] HttpRequest req)
        {
            _logger.LogInformation("Activate Profile triggered");

            // Read the body of the request and return the response
            _logger.LogInformation("Content type is {0}", req.ContentType);
            

            return new OkObjectResult(new
            {
                version = "1.0.0.0",
                action = "Continue"
            });
        }
    }
}
