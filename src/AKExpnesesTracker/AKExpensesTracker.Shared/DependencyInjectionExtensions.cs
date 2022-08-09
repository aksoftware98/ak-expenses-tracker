using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using AKExpensesTracker.Shared.Validators;
using AKExpensesTracker.Shared.DTOs;

namespace AKExpensesTracker.Shared;

public static class DependencyInjectionExtensions
{
    public static void AddValidators(this IServiceCollection services)
    {
        // Automatic way 
        services.AddValidatorsFromAssemblyContaining<WalletDtoValidator>();

        // Manual way
        //services.AddScoped<IValidator<WalletDto>, WalletDtoValidator>(); 
    }
}
