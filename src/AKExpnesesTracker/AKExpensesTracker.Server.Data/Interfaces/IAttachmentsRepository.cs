namespace AKExpensesTracker.Server.Data.Interfaces
{
    public interface IAttachmentsRepository
    {
        Task AddAsync(Attachment attachment);
    }
}