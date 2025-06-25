namespace JumpStartCS.Orleans.Grains.Abstractions
{
    public interface IATMWithdrawerGrain : IGrainWithIntegerKey
    {
        public Task Withdraw(Guid atmId, Guid checkingAccountId, decimal amount);
    }
}
