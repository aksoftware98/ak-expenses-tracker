using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json; 
using System.Threading.Tasks;

namespace AKExpensesTracker.Server.Data.Models
{
	public class Transaction
	{

        public Transaction()
        {
			Id = Guid.NewGuid().ToString();
			CreationDate = DateTime.UtcNow;
			UserId = string.Empty;
			UserIdYear = string.Empty;
			Category = string.Empty;
			WalletId = string.Empty;
        }

        [JsonProperty("id")]
		public string Id { get; private set; }

		[JsonProperty("description")]
		public string? Description { get; private set; }

		[JsonProperty("isIncome")]
		public bool IsIncome { get; private set; }

		[JsonProperty("amount")]
		public decimal Amount { get; private set; }

		[JsonProperty("creationDate")]
		public DateTime CreationDate { get; private set; }

		[JsonProperty("modificationDate")]
		public DateTime? ModificationDate { get; private set; }

		[JsonProperty("category")]
		public string Category { get; private set; }

		[JsonProperty("tags")]
		public string[]? Tags { get; private set; }

		[JsonProperty("attachments")]
		public string[]? Attachments { get; private set; }

		private string _userId = string.Empty; 
		[JsonProperty("userId")]
		public string UserId
		{
			get => _userId; 
			private set
			{
				_userId = value; 
				UserIdYear = $"{_userId}-{CreationDate.Year}";
			}
		}

		[JsonProperty("userIdYear")]
		public string UserIdYear { get; private set; } // 333333-2023

		[JsonProperty("walletId")]
		public string WalletId { get; private set; }

		public static Transaction Create(string walletId, 
										 string userId, 
										 decimal amount,
										 string category,
										 bool isIncome,
										 string? description = null,
										 string[]? tags = null,
										 string[]? attachments = null)
		{
			return new Transaction
			{
				WalletId = walletId,
				UserId = userId,
				Amount = amount,
				Category = category,
				IsIncome = isIncome,
				Description = description,
				Tags = tags,
				Attachments = attachments,
				ModificationDate = DateTime.UtcNow
			};
		}

		public void Update(bool isIncome,
						   decimal amount,
						   string category,
						   string? description = null,
						   string[]? tags = null,
						   string[]? attachments = null)
		{
			IsIncome = isIncome;
			Amount = amount;
			Category = category;
			Description = description;
			Tags = tags;
			Attachments = attachments;
			ModificationDate = DateTime.UtcNow;
		}

	}
}
