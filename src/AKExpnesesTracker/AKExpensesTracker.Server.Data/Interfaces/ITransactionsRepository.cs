namespace AKExpensesTracker.Server.Data.Interfaces
{
	public interface ITransactionsRepository
    {
        Task DeleteAsync(Transaction transaction);

        Task CreateAsync(Transaction transaction);

        Task UpdateAsync(Transaction transaction);
    }
}