namespace AKExpensesTracker.Server.Data.Interfaces
{
	public interface ITransactionsRepository
    {
        Task<Transaction> GetByIdAsync(string id, string userId, int year);

        Task DeleteAsync(Transaction transaction);

        Task CreateAsync(Transaction transaction);

        Task UpdateAsync(Transaction transaction);
    }
}