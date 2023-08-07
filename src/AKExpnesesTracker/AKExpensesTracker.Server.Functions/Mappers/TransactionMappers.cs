using AKExpensesTracker.Server.Data.Models;
using AKExpensesTracker.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKExpensesTracker.Server.Functions.Mappers
{
	public static class TransactionMappers
	{

		public static TransactionDto ToTransactionDto(this Transaction transaction)
			=> new TransactionDto
			{
				Id = transaction.Id,
				Amount = transaction.Amount,
				Category = transaction.Category,
				CreationDate = transaction.CreationDate, // TODO: Handle the time zone of the client
				Description = transaction.Description,
				IsIncome = transaction.IsIncome,
				WalletId = transaction.WalletId,
				Tags = transaction.Tags,
				Attachments = transaction.Attachments
			};

	}
}
