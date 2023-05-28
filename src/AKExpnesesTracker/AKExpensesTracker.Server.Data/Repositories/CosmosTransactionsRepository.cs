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
    public async Task<IEnumerable<Transaction>> ListByUserIdAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        var queryText = $"SELECT * FROM c WHERE c.userId = @userId";
        var query = new QueryDefinition(queryText)
                            .WithParameter("@userId", userId);

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

}