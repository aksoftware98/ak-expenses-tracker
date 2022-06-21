using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKExpensesTracker.Server.Data
{
    public static class DependencyInjectionExtensions
    {

        public static void AddCosmosDbClient(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton(sp => new CosmosClient(connectionString));
        }

    }
}
