using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKExpensesTracker.Server.Data.Models
{
    public class Attachment
    {

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("uploadedByUserId")]
        public string? UploadedByUserId { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("uploadingDate")]
        public DateTime UploadingDate { get; set; }

    }
}
