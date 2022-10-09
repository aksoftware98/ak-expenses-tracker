using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data.Interfaces;
using AKExpensesTracker.Server.Functions.Services;
using AKExpensesTracker.Shared.Responses;
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
    public class UploadAttachment
    {
        private readonly ILogger<UploadAttachment> _logger;
        private readonly IStorageService _storageService;
        private readonly IAttachmentsRepository _attachmentsRepo;
        public UploadAttachment(ILogger<UploadAttachment> log, 
                                IStorageService storageService, 
                                IAttachmentsRepository attachmentsRepo)
        {
            _logger = log;
            _storageService = storageService;
            _attachmentsRepo = attachmentsRepo;
        }

        [FunctionName("UploadAttachment")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("UploadAttachment has been triggered");
            var userId = "userId"; // TODO: Fetch the token 
            
            // Read the file from the form     
            var file = req.Form.Files["File"];
            if (file == null)
                return new BadRequestObjectResult(new ApiErrorResponse("File is required"));

            // TODO: Call the Microsoft Conputer Vision API to make sure it's a document image

            // Save the file and retrieve the URL 
            string url = string.Empty; 
            try
            {
                url = await _storageService.SaveFileAsync(file.OpenReadStream(), file.FileName); 
            }
            catch (NotSupportedException ex)
            {
                return new BadRequestObjectResult(new ApiErrorResponse("File is required"));
            }

            // Save the URL in the database 
            await _attachmentsRepo.AddAsync(new Data.Models.Attachment
            {
                Id = Guid.NewGuid().ToString(),
                UploadedByUserId = userId,
                UploadingDate = DateTime.UtcNow, // TODO: Use the Azure Functions provided UTC time
                Url = url,
            });

            return new OkObjectResult(new ApiSuccessResponse<string>($"Attachment has been uploaded", url));
        }
    }
}

