namespace AKExpensesTracker.Server.Data.Interfaces
{
	public interface ITransactionsRepository
    {
        Task<Transaction> GetByIdAsync(string id, string userId, int year);

        Task DeleteAsync(Transaction transaction);

        Task CreateAsync(Transaction transaction);

        Task UpdateAsync(Transaction transaction);

        Task<IEnumerable<Transaction>> ListByUserIdAsync(string userId, int year, IEnumerable<string> walletIds, DateTime? minDate = null, DateTime? maxDate = null);
    }
}