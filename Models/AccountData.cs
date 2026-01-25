namespace BankingApp.Models;

internal class AccountData
{
    public int AccountNumber { get; set; }
    public decimal Balance { get; set; }
    public AccountType AccountType { get; set; }
}