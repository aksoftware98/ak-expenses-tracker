using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKExpensesTracker.Server.Data.Repositories
{
	public class CosmosAttachmentsRepository : IAttachmentsRepository
	{
		private readonly CosmosClient _db;
		private const string DATABASE_NAME = "ExpnesesTrackerDb";
		private const string CONTAINER_NAME = "Attachments";

		public CosmosAttachmentsRepository(CosmosClient db)
		{
			_db = db;
		}

		public async Task AddAsync(Attachment attachment)
		{
			if (attachment == null)
				throw new ArgumentNullException(nameof(attachment));

			var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);
			await container.CreateItemAsync(attachment);
		}

		public async Task<IEnumerable<Attachment>> GetUnusedAttachmentsAsync(int hours)
		{
			var queryText = $"SELECT * FROM c WHERE DateTimeDiff('hour', c.uploadingDate, GetCurrentDateTime()) > @hours";
			var query = new QueryDefinition(queryText)
								.WithParameter("@hours", hours);

			var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);

			var iterator = container.GetItemQueryIterator<Attachment>(query);
			var result = await iterator.ReadNextAsync();
			var attachments = new List<Attachment>();
			if (result.Any())
				attachments.AddRange(result.Resource);
			
			while (result.ContinuationToken != null)
			{
				iterator = container.GetItemQueryIterator<Attachment>(query, result.ContinuationToken);
				result = await iterator.ReadNextAsync();
				attachments.AddRange(result.Resource);
			}

			return attachments;
		}
	}
}
