using System.Globalization;

using BankingApp.Services;
using BankingApp.Models;

var bank = await Bank.Create();
Console.WriteLine("""
                ===========
=============== BANKING APP ===============
                ===========
""");
while (true)
{
    Console.WriteLine("""
        1. Create account
        2. Deposit
        3. Withdraw
        4. Transfer
        5. Check balance
        6. Apply interest
        7. Exit
    """);
    Console.Write("Choose: ");
    switch (Console.ReadKey().KeyChar)
    {
        case '1':
            Console.WriteLine();
            Console.WriteLine("""
            Which type of account do you want to create?
                1: Current account
                2: Savings account
            """);
            Console.Write("Choose: ");
            char accountTypeInput = Console.ReadKey().KeyChar;
            Console.WriteLine();
            if (accountTypeInput != '1' && accountTypeInput != '2')
            {
                Console.WriteLine("Invalid account type input");
                continue;
            }
            var accountType = accountTypeInput switch
            {
                '1' => AccountType.Current,
                '2' => AccountType.Savings,
                _ => throw new InvalidOperationException("Invalid acount type")
            };
            var accountNumber = await bank.CreateAccount(accountType);
            Console.WriteLine($"Successfully created a new {accountType} account! Your account number is {accountNumber}");
            break;
        case '2':
            Console.WriteLine();
            var depositBankAccount = GetAccount("your");
            if (depositBankAccount == null)
            {
                continue;
            }
            if (!GetAmount("deposit", out decimal depositAmount))
            {
                continue;
            }
            depositBankAccount.Deposit(depositAmount);
            Console.WriteLine($"Successfully deposited {depositAmount:C2}");
            break;
        case '3':
            Console.WriteLine();
            var withdrawBankAccount = GetAccount("your");
            if (withdrawBankAccount == null)
            {
                continue;
            }
            if (!GetAmount("withdraw", out decimal withdrawAmount))
            {
                continue;
            }
            if (!withdrawBankAccount.TryWithdraw(withdrawAmount))
            {
                Console.WriteLine("Withdrawal failed");
                continue;
            }
            Console.WriteLine($"Successfully withdrew {withdrawAmount:C2}");
            break;
        case '4':
            Console.WriteLine();
            var senderBankAccount = GetAccount("your");
            if (senderBankAccount == null)
            {
                continue;
            }
            var recipientBankAccount = GetAccount("recipient's");
            if (recipientBankAccount == null)
            {
                continue;
            }
            if (Equals(senderBankAccount, recipientBankAccount))
            {
                Console.WriteLine("Cannot tranfer to the same account");
                continue;
            }
            if (!GetAmount("transfer", out decimal transferAmount))
            {
                continue;
            }
            if (!await bank.Transfer(senderBankAccount, recipientBankAccount, transferAmount))
            {
                Console.WriteLine("Transfer failed");
                continue;
            }
            break;
        case '5':
            Console.WriteLine();
            var balanceAccount = GetAccount("your");
            if (balanceAccount == null)
            {
                continue;
            }
            Console.WriteLine($"Your account balance is {balanceAccount.Balance:C2}");
            break;
        case '6':
            Console.WriteLine();
            var interestAccount = GetAccount("your");
            if (interestAccount == null)
            {
                continue;
            }
            if (!await bank.ApplyInterest(interestAccount))
            {
                Console.WriteLine("Interest can only be applied to savings account");
                continue;
            }
            Console.WriteLine("Successfully applied interest");
            break;
        case '7':
            return;
        default:
            Console.WriteLine();
            Console.WriteLine("Invalid input. Please try again");
            continue;
    }
}
BankAccount? GetAccount(string name)
{
    Console.WriteLine($"Enter {name} account number");
    if (!int.TryParse(Console.ReadLine(), out int accountNumber))
    {
        Console.WriteLine("Invalid account number");
        return null;
    }
    var bankAccount = bank.FindAccount(accountNumber);
    if (bankAccount == null)
    {
        Console.WriteLine("Account number not found");
    }
    return bankAccount;
}
static bool GetAmount(string action, out decimal amount)
{
    Console.WriteLine($"Enter the amount you'd like to {action}");
    if (!decimal.TryParse(Console.ReadLine(),
        NumberStyles.Number,
        CultureInfo.InvariantCulture,
        out amount))
    {
        Console.WriteLine($"Invalid {action} amount");
        return false;
    }
    amount = Math.Round(amount, 2);
    if (amount <= 0)
    {
        Console.WriteLine($"The amount to {action} must be greater than 0");
        return false;
    }
    return true;
}