using System.ComponentModel.DataAnnotations;

namespace HelloDapr.Models
{
    public class DepositModel
    {
        [Required]
        public string WalletId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
