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
		private const string DATABASE_NAME = "ExpensesTrackerDb";
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

		public async Task<IEnumerable<Attachment>> GetByURLsAsync(string[] urls)
		{
			if (urls == null)
				throw new ArgumentNullException(nameof(urls));
			if (!urls.Any())
				return Enumerable.Empty<Attachment>();

			// SELECT * FROM c WHERE c.url in ('https://l....jpg', '')
			var urlsAsString = string.Join(",", urls.Select(x => $"{x}"));
			var queryText = $"SELECT * FROM c WHERE c.url in (@urls)"; 
			var query = new QueryDefinition(queryText)
								.WithParameter("@urls", urlsAsString);

			var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);
			var iterator = container.GetItemQueryIterator<Attachment>(query); 
			var result = await iterator.ReadNextAsync();

			if (result.Any())
				return result.Resource; 

			return Enumerable.Empty<Attachment>();
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

		public async Task DeleteAsync(string id, string uploadedByUserId)
		{
			if (string.IsNullOrWhiteSpace(id))
				throw new ArgumentNullException(nameof(id));

			if (string.IsNullOrWhiteSpace(uploadedByUserId))
				throw new ArgumentNullException(nameof(uploadedByUserId));
			
			var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);
			await container.DeleteItemAsync<Attachment>(id, new PartitionKey(uploadedByUserId));
		}

		public async Task DeleteBatchAsync(IEnumerable<Attachment> attachments)
		{
			if (attachments == null)
				throw new ArgumentNullException(nameof(attachments));

			if (!attachments.Any())
				return; 

			var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);
			var tasks = new List<Task>();
			foreach (var attachment in attachments)
			{
				var deleteTask = container.DeleteItemAsync<Attachment>(attachment.Id, new PartitionKey(attachment.UploadedByUserId));
				tasks.Add(deleteTask);
			}

			await Task.WhenAll(tasks);
		}
	}
}
