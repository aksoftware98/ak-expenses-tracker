using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKExpensesTracker.Shared.DTOs
{
	public class TransactionDto
	{

        public TransactionDto()
        {
			Category = string.Empty; 
			WalletId = string.Empty;
        }

        public string? Id { get; set; }

		public string? Description { get; set; }

		public bool IsIncome { get; set; }

		public decimal Amount { get; set; }

		public DateTime CreationDate { get; set; } // Ready-only

		public string[]? Tags { get; set; }

		public string[]? Attachments { get; set; }

		public string Category { get; set; }

		public string WalletId { get; set; }

	}
}
