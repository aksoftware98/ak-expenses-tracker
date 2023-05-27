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
		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("description")]
		public string? Description { get; set; }

		[JsonPropertyName("isIncome")]
		public bool IsIncome { get; set; }

		[JsonPropertyName("amount")]
		public decimal Amount { get; set; }

		[JsonPropertyName("creationDate")]
		public DateTime CreationDate { get; set; }

		[JsonPropertyName("modificationDate")]
		public DateTime? ModificationDate { get; set; }

		[JsonPropertyName("category")]
		public string Category { get; set; }

		[JsonPropertyName("tags")]
		public string[]? Tags { get; set; }

		[JsonPropertyName("attachments")]
		public string[]? Attachments { get; set; }

		[JsonPropertyName("userId")]
		public string UserId { get; set; }

		[JsonPropertyName("userIdYear")]
		public string UserIdYear { get; set; }
	}
}
