using Dapr;
using Dapr.Client;
using HelloDapr.Common;
using HelloDapr.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelloDapr.Wallets.Controllers
{
    [Route("wallets")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        const string StateStore = "statestore-dev";
        readonly ILogger<WalletsController> _logger;

        public WalletsController(ILogger<WalletsController> logger)
        {
            _logger = logger;
        }

        #region CRUD actions

        [HttpGet("{id}")]
        public ActionResult<Wallet> Get([FromState(StateStore, "id")] StateEntry<Wallet> wallet)
        {
            if (wallet.Value == null)
            {
                return NotFound();
            }

            Response.Headers.ETag = wallet.ETag;
            return wallet.Value;
        }

        [HttpPost]
        public async Task<ActionResult> Post(
            [FromBody] Wallet wallet,
            [FromServices] DaprClient dapr,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!await dapr.TrySaveStateAsync(
                    StateStore,
                    wallet.Id,
                    wallet,
                    string.Empty,
                    stateOptions: new StateOptions { Concurrency = ConcurrencyMode.FirstWrite, Consistency = ConsistencyMode.Strong },
                    cancellationToken: cancellationToken
                    )) return Conflict();

                return NoContent();
            }
            catch (DaprException ex) when (ex.InnerException != null && ex.InnerException as Grpc.Core.RpcException != null && ((Grpc.Core.RpcException)ex.InnerException).StatusCode == Grpc.Core.StatusCode.Aborted)
            {
                // Probably precondition failed because item already exists.
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(
            [FromBody] Wallet wallet,
            [FromServices] DaprClient dapr,
            CancellationToken cancellationToken)
        {
            if (!Request.Headers.IfMatch.Any()) return BadRequest("If-Match header is required for PUT operations.");

            if (!await dapr.TrySaveStateAsync(
                StateStore,
                wallet.Id,
                wallet,
                Request.Headers.IfMatch,
                stateOptions: new StateOptions { Concurrency = ConcurrencyMode.FirstWrite },
                cancellationToken: cancellationToken))
            {
                return Conflict();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(
            string id,
            [FromServices] DaprClient dapr,
            CancellationToken cancellationToken)
        {
            if (!Request.Headers.IfMatch.Any()) return BadRequest("If-Match header is required for DELETE operations.");

            if (!await dapr.TryDeleteStateAsync(
                StateStore,
                id,
                Request.Headers.IfMatch,
                stateOptions: new StateOptions { Concurrency = ConcurrencyMode.FirstWrite },
                cancellationToken: cancellationToken))
            {
                return Conflict();
            }

            return Ok();
        }

        #endregion CRUD actions

        #region Business actions

        [HttpPut("deposit")]
        public async Task<ActionResult> Deposit(
            [FromBody] DepositModel deposit,
            [FromServices] DaprClient dapr,
            CancellationToken cancellationToken)
        {
            (Wallet wallet, string etag) = await dapr.GetStateAndETagAsync<Wallet>(
                StateStore,
                deposit.WalletId,
                consistencyMode: ConsistencyMode.Strong,
                cancellationToken: cancellationToken);

            wallet.Credit(deposit.Amount);

            if (!await dapr.TrySaveStateAsync(
                StateStore,
                deposit.WalletId,
                wallet,
                etag,
                stateOptions: new StateOptions { Concurrency = ConcurrencyMode.FirstWrite, Consistency = ConsistencyMode.Strong },
                cancellationToken: cancellationToken))
            {
                return Conflict();
            }

            return NoContent();
        }

        [HttpPut("withdraw")]
        public async Task<ActionResult> Withdraw(
            [FromBody] WithdrawalModel withdrawal,
            [FromServices] DaprClient dapr,
            CancellationToken cancellationToken)
        {
            (Wallet wallet, string etag) = await dapr.GetStateAndETagAsync<Wallet>(
                StateStore,
                withdrawal.WalletId,
                consistencyMode: ConsistencyMode.Strong,
                cancellationToken: cancellationToken);

            wallet.Debit(withdrawal.Amount);

            if (!await dapr.TrySaveStateAsync(
                StateStore,
                withdrawal.WalletId,
                wallet,
                etag,
                stateOptions: new StateOptions { Concurrency = ConcurrencyMode.FirstWrite, Consistency = ConsistencyMode.Strong },
                cancellationToken: cancellationToken))
            {
                return Conflict();
            }

            return NoContent();
        }

        #endregion
    }
}
