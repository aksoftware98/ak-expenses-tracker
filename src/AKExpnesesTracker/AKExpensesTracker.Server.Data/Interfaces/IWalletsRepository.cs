using AKExpensesTracker.Server.Data.Models;

namespace AKExpensesTracker.Server.Data.Interfaces;

public interface IWalletsRepository
{
    Task<IEnumerable<Wallet>> ListByUserIdAsync(string userId);
}
