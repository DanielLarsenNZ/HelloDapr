using System.ComponentModel.DataAnnotations;

namespace HelloDapr.Wallets
{
    public class Deposit
    {
        [Required]
        public string WalletId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
