using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AKExpensesTracker.Server.Data.Models
{
	public class Transaction
	{

        public Transaction()
        {
            Id = Guid.NewGuid().ToString();
			CreationDate = DateTime.UtcNow;
			Category = string.Empty; 
			UserId = string.Empty; 
			UserIdYear = string.Empty;
        }

		[JsonProperty("id")]
		[JsonPropertyName("id")]
		public string Id { get; private set; }

		[JsonProperty("description")]
		[JsonPropertyName("description")]
		public string? Description { get; private set; }

		[JsonProperty("isIncome")]
		[JsonPropertyName("isIncome")]
		public bool IsIncome { get; private set; }

		[JsonProperty("amount")]
		[JsonPropertyName("amount")]
		public decimal Amount { get; private set; }

		[JsonProperty("creationDate")]
		[JsonPropertyName("creationDate")]
		public DateTime CreationDate { get; private set; }

		[JsonProperty("modificationDate")]
		[JsonPropertyName("modificationDate")]
		public DateTime? ModificationDate { get; private set; }

		[JsonProperty("category")]
		[JsonPropertyName("category")]
		public string Category { get; private set; }

		[JsonProperty("tags")]
		[JsonPropertyName("tags")]
		public string[]? Tags { get; private set; }

		[JsonProperty("attachments")]
		[JsonPropertyName("attachments")]
		public string[]? Attachments { get; private set; }

		private string _userId = string.Empty;

		[JsonProperty("walletId")]
		public string WalletId { get; private set; }

		[JsonProperty("userId")]
		[JsonPropertyName("userId")]
		public string UserId
		{
			get => _userId; 
			private set
			{
				_userId = value;
				UserIdYear = $"{UserId}_{CreationDate.Year}";
			}
		}

		[JsonProperty("userIdYear")]
		[JsonPropertyName("userIdYear")]
		public string UserIdYear { get; private set; }

		public static Transaction Create(string userId, decimal amount, string walletId, string category, bool isIncome, string? description = null, string[]? tags = null, string[]? attachments = null)
		{
			return new Transaction
			{
				UserId = userId,
				Amount = amount,
				Category = category,
				WalletId = walletId,
				IsIncome = isIncome,
				Description = description,
				Tags = tags,
				Attachments = attachments
			};
		}

		public void Update(decimal amount, string category, bool isIncome, string? description = null, string[]? tags = null, string[]? attachments = null)
		{
			Amount = amount;
			Category = category;
			IsIncome = isIncome;
			Description = description;
			Tags = tags;
			Attachments = attachments;
			ModificationDate = DateTime.UtcNow;
		}
	}
}
