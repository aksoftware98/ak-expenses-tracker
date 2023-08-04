using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKExpensesTracker.Shared.DTOs;

public class TransactionDto
{

    public TransactionDto()
    {
		Category = "Other";
		WalletId = string.Empty;
		Attachments = new string[0];
    }

    public string? Id { get; set; }

	public string? Description { get; set; }

	public double Amount { get; set; }

	public DateTime? DateTime { get; set; }

	public string Category { get; set; }

	public string[]? Tags { get; set; }
	public string[]? Attachments { get; set; }

	public bool IsIncome { get; set; }

	public string WalletId { get; set; }
}
