﻿namespace AKExpensesTracker.Server.Data.Interfaces
{
	public interface IAttachmentsRepository
    {
        Task AddAsync(Attachment attachment);

        Task<IEnumerable<Attachment>> GetUnusedAttachmentsAsync(int hours);

		Task DeleteAsync(string id, string uploadedByUserId);
	}
}