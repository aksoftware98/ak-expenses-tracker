using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKExpensesTracker.Server.Data.Repositories
{
	public class CosmosTransactionsRepository : ITransactionsRepository
	{

		private readonly CosmosClient _db;
		private readonly Container _container;
		private const string DATABASE_NAME = "ExpensesTrackerDb";
		private const string CONTAINER_NAME = "Transactions";
		public CosmosTransactionsRepository(CosmosClient db)
		{
			_db = db;
			_container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);
		}


		public async Task<IEnumerable<Transaction>> ListByUserIdAsync(string userId, int year, IEnumerable<string> walletIds, DateTime? minDate = null, DateTime? maxDate = null)
		{
			if (string.IsNullOrWhiteSpace(userId))
				throw new ArgumentNullException(nameof(userId));
			if (year > 9999 || year < 1000)
				throw new ArgumentOutOfRangeException(nameof(year));
			if (walletIds == null || !walletIds.Any())
				throw new ArgumentNullException(nameof(walletIds));

			var queryBuilder = new StringBuilder();
			queryBuilder.Append("SELECT * FROM c WHERE c.userIdYear = @userIdYear AND c.walletId IN (@walletIds)");
			if (minDate != null)
				queryBuilder.Append(" AND c.creationDate >= @minDate");
			if (maxDate != null)
				queryBuilder.Append(" AND c.creationDate <= @maxDate");

			var query = new QueryDefinition(queryBuilder.ToString())
				.WithParameter("@userIdYear", $"{userId}_{year}")
				.WithParameter("@walletIds", string.Join(",", walletIds));

			if (minDate != null)
				query = query.WithParameter("@minDate", minDate.Value);
			if (maxDate != null)
				query = query.WithParameter("@maxDate", maxDate.Value);

			var iterator = _container.GetItemQueryIterator<Transaction>(query);
			var result = await iterator.ReadNextAsync();

			return result;
		}

		public async Task CreateAsync(Transaction transaction)
		{
			if (transaction == null)
				throw new ArgumentNullException(nameof(transaction));

			await _container.CreateItemAsync(transaction);
		}

		public async Task DeleteAsync(Transaction transaction)
		{
			if (transaction == null)
				throw new ArgumentNullException(nameof(transaction));

			await _container.DeleteItemAsync<Transaction>(transaction.Id, new PartitionKey(transaction.UserIdYear));
		}

		public async Task UpdateAsync(Transaction transaction)
		{
			if (transaction == null)
				throw new ArgumentNullException(nameof(transaction));

			await _container.ReplaceItemAsync(transaction, transaction.Id);
		}

		public async Task<Transaction> GetByIdAsync(string id, string userId, int year)
		{
			if (string.IsNullOrWhiteSpace(id))
				throw new ArgumentNullException(nameof(id));

			if (string.IsNullOrWhiteSpace(userId))
				throw new ArgumentNullException(nameof(userId));
			
			var partitionKeyValue = $"{userId}-{year}";
			var result = await _container.ReadItemAsync<Transaction>(id, new PartitionKey(partitionKeyValue));
			return result.Resource;
		}
	}
}
