using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MembershipService.Application.DTOs
{
    public class CancelRequestDto
    {
        [JsonPropertyName("id")]
        public string SubscriptionId { get; set; }
        public string CancellationReason { get; set; }

        [JsonPropertyName("customerEmail")]
        public string CustomerEmail { get; set; } 
    }
}
