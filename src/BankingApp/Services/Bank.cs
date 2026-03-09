using System.Text.Json;

using BankingApp.Models;
namespace BankingApp.Services;

class Bank
{
    private static readonly string FilePath = "accounts.json";
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        IndentSize = 4
    };
    private readonly List<BankAccount> _accounts = [];
    public static async Task<Bank> Create()
    {
        var bank = new Bank();
        var loadedAccounts = await LoadAccounts();
        bank._accounts.AddRange(loadedAccounts);
        return bank;
    }
    public async Task<int> CreateAccount(AccountType accountType)
    {
        int accountNumber = _accounts.Count == 0 ? 1000 : _accounts.Max(a => a.AccountNumber) + 1;
        BankAccount account = accountType == AccountType.Current ? new CurrentAccount(accountNumber) : new SavingsAccount(accountNumber);
        _accounts.Add(account);
        await SaveAccounts(_accounts);
        return accountNumber;
    }
    public BankAccount? FindAccount(int accountNumber) => _accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
    public async Task<bool> Transfer(BankAccount fromAccount, BankAccount toAccount, decimal amount)
    {
        if (!fromAccount.TryWithdraw(amount))
        {
            return false;
        }
        toAccount.Deposit(amount);
        await SaveAccounts(_accounts);
        return true;
    }
    public async Task Deposit(BankAccount account, decimal amount)
    {
        account.Deposit(amount);
        await SaveAccounts(_accounts);
    }
    public async Task<bool> Withdraw(BankAccount account, decimal amount)
    {
        if (!account.TryWithdraw(amount))
        {
            return false;
        }
        await SaveAccounts(_accounts);
        return true;
    }
    public async Task<bool> ApplyInterest(BankAccount account)
    {
        if (!account.TryApplyInterest())
        {
            return false;
        }
        await SaveAccounts(_accounts);
        return true;
    }
    private static async Task SaveAccounts(List<BankAccount> accounts)
    {
        var data = accounts.Select(a => new AccountData
        {
            AccountNumber = a.AccountNumber,
            Balance = a.Balance,
            AccountType = a switch
            {
                SavingsAccount => AccountType.Savings,
                CurrentAccount => AccountType.Current,
                _ => throw new InvalidOperationException("Invalid account type")
            }
        }).ToList();
        var json = JsonSerializer.Serialize(data, Options);
        await File.WriteAllTextAsync(FilePath, json);
    }
    private static async Task<List<BankAccount>> LoadAccounts()
    {
        if (!File.Exists(FilePath))
        {
            return [];
        }
        var json = await File.ReadAllTextAsync(FilePath);
        var data = JsonSerializer.Deserialize<List<AccountData>>(json) ?? [];
        var accounts = new List<BankAccount>();
        foreach (var item in data)
        {
            BankAccount account = item.AccountType switch
            {
                AccountType.Current => new CurrentAccount(item.AccountNumber),
                AccountType.Savings => new SavingsAccount(item.AccountNumber),
                _ => throw new InvalidOperationException("Invalid account type")
            };
            account.Deposit(item.Balance);
            accounts.Add(account);
        }
        return accounts;
    }
}