using System.Text;

namespace AKExpensesTracker.Server.Data.Repositories;

public class CosmosTransactionsRepository : ITransactionsRepository
{

    private readonly CosmosClient _db;
    private const string DATABASE_NAME = "ExpensesTrackerDb";
    private const string CONTAINER_NAME = "Transactions";

    public CosmosTransactionsRepository(CosmosClient db)
    {
        _db = db;
    }

    #region List
    public async Task<IEnumerable<Transaction>> ListByUserIdAsync(string userId, int year, DateTime? minDate, DateTime? maxDate, IEnumerable<string> walletIds, string? pageToken = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));
        if (year > 9999 || year < 1000)
			throw new ArgumentOutOfRangeException(nameof(year));
        if (walletIds == null || !walletIds.Any())
            throw new ArgumentNullException(nameof(walletIds));

        var stringBuilder = new StringBuilder();  
        stringBuilder.Append("SELECT * FROM c WHERE c.userIdYear = @userIdYear");
        if (minDate != null)
            stringBuilder.Append(" AND c.date >= @minDate");
        if (maxDate != null)
            stringBuilder.Append(" AND c.date <= @maxDate");
        stringBuilder.Append(" AND c.walletId IN (@walletIds)");

        var query = new QueryDefinition(stringBuilder.ToString())
                            .WithParameter("@userIdYear", $"{userId}_{year}");
        if (minDate != null)
			query.WithParameter("@minDate", minDate);
        if (maxDate != null)
            query.WithParameter("@maxDate", maxDate);
        query.WithParameter("@walletIds", string.Join(",", walletIds));

        var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);

        var iterator = container.GetItemQueryIterator<Transaction>(query);
        var result = await iterator.ReadNextAsync();

        return result.Resource;
    }
    #endregion

    #region Create 
    public async Task CreateAsync(Wallet wallet)
    {
        if (wallet == null)
            throw new ArgumentNullException(nameof(wallet));

        var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);

        await container.CreateItemAsync(wallet);
    }

	public async Task CreateAsync(Transaction transaction)
	{
		if (transaction == null)
			throw new ArgumentNullException(nameof(transaction));

		var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);

		await container.CreateItemAsync(transaction);
	}

	#endregion

	#region Get By Id  
    public async Task<Transaction> GetByIdAsync(string id, string userId, int year)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));
        if (userId == null)
			throw new ArgumentNullException(nameof(userId));    

        var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);
        try
        {
			var item = await container.ReadItemAsync<Transaction>(id, new PartitionKey($"{userId}_{year}"));
			return item;
		}
        catch (Exception ex)
        {
            throw;
        }
       
    }
	#endregion

	#region Delete 
    public async Task DeleteAsync(Transaction transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);

        await container.DeleteItemAsync<Transaction>(transaction.Id, new PartitionKey(transaction.UserIdYear));
    }
	#endregion 

}