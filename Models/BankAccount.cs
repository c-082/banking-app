namespace BankingApp.Models;

abstract class BankAccount(int accountNumber)
{
    public int AccountNumber { get; } = accountNumber;
    public decimal Balance { get; protected set; } = 0;

    public void Deposit(decimal amount) => Balance += amount;
    public abstract bool Withdraw(decimal amount);
    public virtual bool ApplyInterest() => false;
}