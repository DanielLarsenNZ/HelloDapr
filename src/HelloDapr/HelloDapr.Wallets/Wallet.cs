using System.ComponentModel.DataAnnotations;

namespace HelloDapr.Wallets
{
    public class Wallet
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Balance { get; set; }
    }
}
