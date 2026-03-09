namespace BankingApp.Models;

class CurrentAccount(int accountNumber) : BankAccount(accountNumber)
{
    private static readonly decimal OverdraftLimit = -1000m;
    public override bool TryWithdraw(decimal amount)
    {
        if (Balance - amount < OverdraftLimit || amount <= 0)
        {
            return false;
        }
        Balance -= amount;
        return true;
    }
}