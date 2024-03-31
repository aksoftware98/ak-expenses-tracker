using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data.Interfaces;
using AKExpensesTracker.Server.Functions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;

namespace AKExpensesTracker.Server.Functions
{
	public class DeleteUnusedAttachments
	{

		private readonly IStorageService _storageService;
		private readonly IAttachmentsRepository _attachmentsRepo;

		public DeleteUnusedAttachments(IStorageService storageService,
									   IAttachmentsRepository attachmentsRepo)
		{
			_storageService = storageService;
			_attachmentsRepo = attachmentsRepo;
		}

		[Function("DeleteUnusedAttachments")]
		public async Task Run([TimerTrigger("0 0 */6 * * *")] TimerInfo myTimer, ILogger log)
		{
			log.LogInformation($"Delete Unused Attachemnts triggerd at {DateTime.Now}");
			// Retireve the files to be deleted 
			var attachments = await _attachmentsRepo.GetUnusedAttachmentsAsync(6);

			if (attachments.Any())
			{
				log.LogInformation($"{attachments.Count()} attachments have been found to be deleted");
				int deletedCount = 0;
				foreach (var item in attachments)
				{
					// Remove it from the storage
					var fileName = Path.GetFileName(item.Url);
					try
					{
						await _storageService.DeleteFileAsync(fileName);
						await _attachmentsRepo.DeleteAsync(item.Id, item.UploadedByUserId);
						deletedCount++;
					}
					catch (Exception ex)
					{
						log.LogError($"Error in deleting the file {fileName}", ex);
					}
				}
				log.LogInformation($"{deletedCount}/{attachments.Count()}  have been deleted successfully!");
			}
			else
			{
				log.LogInformation("No attachments to be deleted have been found");
			}

		}
	}
}
