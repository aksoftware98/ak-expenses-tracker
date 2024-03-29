﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data;
using AKExpensesTracker.Server.Functions.Services;
using AKExpensesTracker.Shared;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AKEpensesTracker.Server.Functions.Startup))]
namespace AKEpensesTracker.Server.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = builder.GetContext().Configuration;

            builder.Services.AddCosmosDbClient(config["CosmosDbConnectionString"]);
            builder.Services.AddRepositories();
            builder.Services.AddValidators();

            builder.Services.AddScoped(sp => new ComputerVisionClient(new ApiKeyServiceClientCredentials(config["ComputerVisionApiKey"]))
            {
                Endpoint = config["ComputerVisionEndpoint"]
            });

            builder.Services.AddScoped<IImageAnalyzer, AzureComputerVisionImageAnalyzerService>();

            builder.Services.AddScoped<IStorageService>(sp => new AzureBlobStorageService(config["AzureWebJobsStorage"]));
        }
    }
}
