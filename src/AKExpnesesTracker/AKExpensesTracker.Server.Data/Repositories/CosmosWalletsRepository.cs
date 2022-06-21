namespace AKExpensesTracker.Server.Data.Repositories;

public class CosmosWalletsRepository : IWalletsRepository
{

    private readonly CosmosClient _db;
    private const string DATABASE_NAME = "ExpnesesTrackerDb";
    private const string CONTAINER_NAME = "Wallets";

    public CosmosWalletsRepository(CosmosClient db)
    {
        _db = db;
    }

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
}
