using AKExpensesTracker.Shared.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKExpensesTracker.Shared.Validators;

public class TransactionDtoValidator : AbstractValidator<TransactionDto>
{

    public TransactionDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("Amount cannot be negative");

        RuleFor(x => x.WalletId)
            .NotEmpty().WithMessage("Wallet is required");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required");

        RuleFor(x => x.DateTime)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Date cannot be in the future")
            .When(x => x.DateTime != null);
    }

}
