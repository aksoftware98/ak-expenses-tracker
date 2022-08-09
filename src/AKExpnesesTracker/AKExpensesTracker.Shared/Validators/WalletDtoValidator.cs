using AKExpensesTracker.Shared.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKExpensesTracker.Shared.Validators
{
    public class WalletDtoValidator : AbstractValidator<WalletDto>
    {
        public WalletDtoValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name must be less than 100 characters");

            RuleFor(p => p.Currency) // USD EUR AED .. etc
                .NotEmpty()
                .WithMessage("Currency is required")
                .Length(3)
                .WithMessage("Currency most be 3 characters length");

            RuleFor(p => p.Swift)
                .Length(8, 11)
                .When(p => !string.IsNullOrWhiteSpace(p.Swift))
                .WithMessage("Swift must be between 8 and 11 characters");

            RuleFor(p => p.Iban)
                .MaximumLength(34)
                .When(p => !string.IsNullOrWhiteSpace(p.Iban))
                .WithMessage("IBAN must be less than 35 characters"); 
        }
    }
}
