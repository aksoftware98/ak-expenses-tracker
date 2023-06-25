namespace AKExpensesTracker.Server.Data.Interfaces;

public interface ITransactionsRepository
{
	Task CreateAsync(Transaction wallet);

	Task<Transaction> GetByIdAsync(string id, string userId, int year);

	Task DeleteAsync(Transaction transaction);
}
