namespace AKExpensesTracker.Server.Data.Repositories;

public class CosmosWalletsRepository : IWalletsRepository
{

    private readonly CosmosClient _db;
    private const string DATABASE_NAME = "ExpensesTrackerDb";
    private const string CONTAINER_NAME = "Wallets";

    public CosmosWalletsRepository(CosmosClient db)
    {
        _db = db;
    }

    #region List
    public async Task<IEnumerable<Wallet>> ListByUserIdAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        var queryText = $"SELECT * FROM c WHERE c.userId = @userId";
        var query = new QueryDefinition(queryText)
                            .WithParameter("@userId", userId);

        var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);

        var iterator = container.GetItemQueryIterator<Wallet>(query);
        var result = await iterator.ReadNextAsync();

        return result.Resource;
    }
    #endregion

    #region Get By Id 
    public async Task<Wallet?> GetByIdAsync(string walletId, string userId)
    {
        if (string.IsNullOrWhiteSpace(walletId))
            throw new ArgumentNullException(nameof(walletId));
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        var queryText = $"SELECT * FROM c WHERE c.id = @id AND c.userId = @userId";
        var query = new QueryDefinition(queryText)
                            .WithParameter("@id", walletId)
                            .WithParameter("@userId", userId);

        // TODO: Resolve the name of the database 
        var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);

        var iterator = container.GetItemQueryIterator<Wallet>(query);
        var result = await iterator.ReadNextAsync();

        return result.Resource.FirstOrDefault(); 
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
    #endregion

    #region Update 
    public async Task UpdateAsync(Wallet wallet)
    {
        if (wallet == null)
            throw new ArgumentNullException(nameof(wallet));

        var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);

        await container.ReplaceItemAsync(wallet, wallet.Id);
    }
    #endregion 

    public async Task UpdateBalanceAsync(string walletId, string userId, double amount)
    {
		if (string.IsNullOrWhiteSpace(walletId))
			throw new ArgumentNullException(nameof(walletId));
		if (string.IsNullOrWhiteSpace(userId))
			throw new ArgumentNullException(nameof(userId));

        if (amount == 0)
			return;

        var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);
        var patchOperations = new[]
        {
            PatchOperation.Increment("/balance", amount),
        };

        await container.PatchItemAsync<Wallet>(walletId, new PartitionKey(userId), patchOperations);
	}
}
