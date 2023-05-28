namespace AKExpensesTracker.Server.Data.Interfaces;

public interface ITransactionsRepository
{
	Task CreateAsync(Transaction wallet);
}
