namespace BankingApp.Models;

internal class SavingsAccount(int accountNumber) : BankAccount(accountNumber)
{
    internal override bool Withdraw(decimal amount)
    {
        if (amount > Balance || amount <= 0)
            return false;
        Balance -= amount;
        return true;
    }
    internal override bool ApplyInterest()
    {
        Balance += 0.05m * Balance;
        return true;
    }
}