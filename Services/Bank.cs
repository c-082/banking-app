using System.Text.Json;

class Bank
{
    private readonly string filePath = "accounts.json";
    readonly List<BankAccount> accounts = [];
    public static async Task<Bank> CreateAsync()
    {
        var bank = new Bank();
        var loadedAccounts = await bank.LoadAccounts();
        bank.accounts.AddRange(loadedAccounts);
        return bank;
    }
    
    public async Task<int> CreateAccount(AccountType accountType)
    {
        int accountNumber = accounts.Count == 0 ? 1000 : accounts.Max(a => a.AccountNumber) + 1;
        BankAccount account = accountType == AccountType.Current ? new CurrentAccount(accountNumber) : new SavingsAccount(accountNumber);
        accounts.Add(account);
        await SaveAccounts(accounts);
        return accountNumber;
    }
    public BankAccount? FindAccount(int accountNumber) => accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
    public async Task<bool> Transfer(BankAccount fromAccount, BankAccount toAccount, decimal amount)
    {
        if(!fromAccount.Withdraw(amount))
            return false;
        toAccount.Deposit(amount);
        await SaveAccounts(accounts);
        return true;
    }
    public async Task<bool> ApplyInterest(BankAccount account)
    {
        if (!account.ApplyInterest())
            return false;
        await SaveAccounts(accounts);
        return true;
    }
    private async Task SaveAccounts(List<BankAccount> accounts)
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
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        await File.WriteAllTextAsync(filePath, json);
    }
    private async Task<List<BankAccount>> LoadAccounts()
    {
        if(!File.Exists(filePath))
            return [];
        var json = await File.ReadAllTextAsync(filePath);
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