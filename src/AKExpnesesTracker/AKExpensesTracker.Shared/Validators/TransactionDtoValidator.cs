using AKExpensesTracker.Shared.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKExpensesTracker.Shared.Validators
{
	public class TransactionDtoValidator : AbstractValidator<TransactionDto>
	{

        public TransactionDtoValidator()
        {
             RuleFor(p => p.Description)
				.NotEmpty()
				.MaximumLength(500);

            RuleFor(p => p.Amount)
                .GreaterThan(0)
                .WithMessage("Amount cannot be negative");

            RuleFor(p => p.Category)
                .NotEmpty()
				.MaximumLength(50)
                .WithMessage("Category is required");

            RuleFor(p => p.WalletId)
                .NotEmpty()
                .WithMessage("Wallet is required");

        }

    }
}
