namespace BankingApp.Models;

internal abstract class BankAccount(int accountNumber)
{
    protected internal int AccountNumber { get; } = accountNumber;
    protected internal decimal Balance { get; private protected set; } = 0;

    public void Deposit(decimal amount) => Balance += amount;
    public abstract bool TryWithdraw(decimal amount);
    public virtual bool TryApplyInterest() => false;
}