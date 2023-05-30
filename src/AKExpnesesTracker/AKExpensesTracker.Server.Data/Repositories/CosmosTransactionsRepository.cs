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
	}
}
