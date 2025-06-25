using JumpStartCS.Orleans.Grains.Abstractions;
using JumpStartCS.Orleans.Grains.State;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Runtime;
using Orleans.Transactions.Abstractions;

namespace JumpStartCS.Orleans.Grains.Grains
{
    public class ATMWithdrawerGrain : Grain, IATMWithdrawerGrain
    {
        private readonly ILogger<AtmGrain> _logger;
		private readonly ITransactionClient _transactionClient;

        public ATMWithdrawerGrain(
            ILogger<AtmGrain> logger,
		    ITransactionClient transactionClient
		) {
            _logger = logger;
			_transactionClient = transactionClient;
        }

        public async Task Withdraw(Guid atmId, Guid checkingAccountId, decimal amount)
        {
			await _transactionClient.RunTransaction(TransactionOption.Create, async () =>
			{
				var atmGrain = GrainFactory.GetGrain<IAtmGrain>(atmId);

				var checkingAccountGrain = GrainFactory.GetGrain<ICheckingAccountGrain>(checkingAccountId);

				await atmGrain.Withdraw(checkingAccountId, amount);

				await checkingAccountGrain.Debit(amount);
			});
        }
    }
}
