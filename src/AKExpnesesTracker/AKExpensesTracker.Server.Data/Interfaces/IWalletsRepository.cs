using AKExpensesTracker.Server.Data.Models;

namespace AKExpensesTracker.Server.Data.Interfaces;

public interface IWalletsRepository
{
    Task<IEnumerable<Wallet>> ListByUserIdAsync(string userId);
    Task<Wallet?> GetByIdAsync(string walletId, string userId);
    Task CreateAsync(Wallet wallet);
    Task UpdateAsync(Wallet wallet);
    Task UpdateWalletBalanceAsync(string walletId, string userId, decimal newBalance);
}
