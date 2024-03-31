using AKExpensesTracker.Server.Functions.Services;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
		var config = context.Configuration;

		services.AddCosmosDbClient(config["CosmosDbConnectionString"]);
		services.AddRepositories();
		services.AddValidators();

		services.AddScoped(sp => new ComputerVisionClient(new ApiKeyServiceClientCredentials(config["ComputerVisionApiKey"]))
		{
			Endpoint = config["ComputerVisionEndpoint"]
		});

		services.AddScoped<IImageAnalyzer, AzureComputerVisionImageAnalyzerService>();

		services.AddScoped<IStorageService>(sp => new AzureBlobStorageService(config["AzureWebJobsStorage"]));
	})
    .Build();

host.Run();
