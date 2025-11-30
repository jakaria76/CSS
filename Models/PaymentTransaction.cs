using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSS.Models
{
    public class PaymentTransaction
    {
        [Key]
        public int Id { get; set; }

        // Event (required)
        [Required]
        public int EventId { get; set; }

        // EventRegistration created AFTER payment success
        public int? RegistrationId { get; set; }
        public EventRegistration? Registration { get; set; }

        [Required, MaxLength(100)]
        public string TranId { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? RequestId { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = "BDT";

        [Required]
        public string PaymentGateway { get; set; } = "aamarpay";

        [Required]
        public string Status { get; set; } = "Pending";

        public string? GatewayResponse { get; set; }

        // payer info
        public string? PayerFullName { get; set; }
        public string? PayerMobile { get; set; }
        public string? PayerEmail { get; set; }
        public string? PayerDataJson { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
