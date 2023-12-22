using System.ComponentModel.DataAnnotations;

namespace HelloDapr.Models
{
    public class PaymentModel
    {
        [Required]
        public string PartyWalletId  { get; set; }
        [Required]
        public string CounterpartyWalletId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
