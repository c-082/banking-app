namespace BankingApp.Models;

internal abstract class BankAccount(int accountNumber)
{
    protected internal int AccountNumber { get; } = accountNumber;
    protected internal decimal Balance { get; private protected set; } = 0;

    internal void Deposit(decimal amount) => Balance += amount;
    internal abstract bool Withdraw(decimal amount);
    internal virtual bool ApplyInterest() => false;
}