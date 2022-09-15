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

    }
}
