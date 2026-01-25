namespace BankingApp.Models;

class SavingsAccount(int accountNumber) : BankAccount(accountNumber)
{
    public override bool Withdraw(decimal amount)
    {
        if (amount > Balance || amount <= 0)
            return false;
        Balance -= amount;
        return true;
    }
    public override bool ApplyInterest()
    {
        Balance += 0.05m * Balance;
        return true;
    }
}