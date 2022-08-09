using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data;
using AKExpensesTracker.Shared;
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
        }
    }
}
