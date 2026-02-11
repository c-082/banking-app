namespace BankingApp.Models;

internal class SavingsAccount(int accountNumber) : BankAccount(accountNumber)
{
    public override bool TryWithdraw(decimal amount)
    {
        if (amount > Balance || amount <= 0)
        {
            return false;
        }
        Balance -= amount;
        return true;
    }
    public override bool TryApplyInterest()
    {
        Balance += 0.05m * Balance;
        return true;
    }
}