using System.ComponentModel.DataAnnotations;

namespace HelloDapr.Models
{
    public class Wallet
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Balance { get; set; }

        #region Domain model object (DMO) methods

        public bool HasSufficientFundsForPaymentWithdrawal(decimal paymentAmount)
        {
            if (paymentAmount <= 0) throw new InvalidOperationException("Payment amount must be greater than zero.");
            return Balance - paymentAmount >= 0;
        }

        public void Debit(decimal amountToWithdraw)
        {
            if (amountToWithdraw <= 0) throw new InvalidOperationException("Withdrawal amount must be greater than zero.");
            Balance -= amountToWithdraw;
        }

        public void Credit(decimal amountToDeposit)
        {
            if (amountToDeposit <= 0) throw new InvalidOperationException("Deposity amount must be greater than zero.");
            Balance += amountToDeposit;
        }

        #endregion
    }
}
