namespace BankingApp.Models;

internal class CurrentAccount(int accountNumber) : BankAccount(accountNumber)
{
    private static readonly decimal overdraftLimit = -1000m;
    internal override bool Withdraw(decimal amount)
    {
        if (Balance - amount < overdraftLimit || amount <= 0)
            return false;
        Balance -= amount;
        return true;
    }
}