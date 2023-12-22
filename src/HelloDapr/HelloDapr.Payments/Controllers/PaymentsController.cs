using Dapr.Client;
using HelloDapr.Common;
using HelloDapr.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelloDapr.Payments.Controllers
{
    [Route("payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        const string StateStore = "statestore-dev";
        readonly ILogger<PaymentsController> _logger;

        public PaymentsController(ILogger<PaymentsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> Post(
            [FromBody] PaymentModel payment,
            [FromServices] DaprClient dapr,
            CancellationToken cancellationToken)
        {
            // 0. Look up party wallet
            Wallet partyWallet;
            try
            {
                partyWallet = await dapr.InvokeMethodAsync<string, Wallet>(
                    HttpMethod.Get,
                    "hellodapr-wallets",
                    $"wallets/{payment.PartyWalletId}",
                    payment.PartyWalletId,
                    cancellationToken: cancellationToken);

                if (partyWallet == null) return NotFound($"Party wallet {payment.PartyWalletId} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving wallet {payment.PartyWalletId}.");
                throw;
            }


            // 1. Look up counterparty wallet
            Wallet counterpartyWallet;
            try
            {
                counterpartyWallet = await dapr.InvokeMethodAsync<string, Wallet>(
                    HttpMethod.Get,
                    "hellodapr-wallets",
                    $"wallets/{payment.CounterpartyWalletId}",
                    payment.CounterpartyWalletId,
                    cancellationToken: cancellationToken);

                if (counterpartyWallet == null) return NotFound($"Counterparty wallet {payment.CounterpartyWalletId} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving wallet {payment.CounterpartyWalletId}.");
                throw;
            }

            // 2. Check wallet balance
            if (!partyWallet.HasSufficientFundsForPaymentWithdrawal(payment.Amount))
            {
                return BadRequest("Insufficient funds.");
            }

            // 3. Anti-money laundering (AML) check
            try
            {
                bool confirmation = await dapr.InvokeMethodAsync<PaymentModel, bool>("hellodapr-aml", "aml/check", payment, cancellationToken: cancellationToken);
                if (confirmation == false) return BadRequest("AML check failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AML check error.");
                throw;
            }

            // 4. Withdraw
            try
            {
                var withdrawal = new WithdrawalModel
                {
                    Amount = payment.Amount,
                    Description = payment.Description,
                    WalletId = payment.PartyWalletId
                };

                await dapr.InvokeMethodAsync(
                    HttpMethod.Put,
                    "hellodapr-wallets",
                    "wallets/withdraw",
                    withdrawal,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error withdrawing funds.");
                throw;
            }

            // 5. Deposit
            try
            {
                var deposit = new DepositModel
                {
                    Amount = payment.Amount,
                    Description = payment.Description,
                    WalletId = payment.CounterpartyWalletId
                };

                await dapr.InvokeMethodAsync(
                    HttpMethod.Put,
                    "hellodapr-wallets",
                    "wallets/deposit",
                    deposit,
                    cancellationToken: cancellationToken);

                // 6. Send transaction
                await dapr.PublishEventAsync("servicebus-pubsub-dev", "payments", payment);

            }
            catch (Exception ex)
            {
                // Deposit or transaction failed! Compensate
                _logger.LogError(ex, $"Error. Compensating transaction.");

                var compensation = new DepositModel
                {
                    Amount = payment.Amount,
                    Description = payment.Description + " - payment failed compensation",
                    WalletId = payment.PartyWalletId
                };

                await dapr.InvokeMethodAsync(
                    HttpMethod.Put,
                    "hellodapr-wallets",
                    "wallets/deposit",
                    compensation,
                    cancellationToken: cancellationToken);

                throw;
            }

            return NoContent();
        }
    }
}
