using System.Diagnostics;
using System.Security.Principal;

public class Program
{
    public static void Main(string[] args)
    {
        BankInterface.PrintWelcomeScreen();
    }
}
public static class BankInterface
{
    public static void PrintUserInterface(Account account)
    {
        Console.Clear();
        Console.WriteLine("Home >> Login >> My Account");
        Console.WriteLine($"Welcome, {account.ownerName}!");
        Console.WriteLine($"Your current balance: {account.balance} €");

        Console.WriteLine("1. Transfer Money");
        Console.WriteLine("2. Logout");

        string? option = Console.ReadLine();
        switch (option)
        {

            case "1":
                PrintTransferMoney(account);
                break;
            case "2":
                PrintWelcomeScreen();
                break;

            default:
                PrintUserInterface(account);
                break;
        }
    }
    private static void PrintTransferMoney(Account sender)
    {
        Console.Clear();
        Console.WriteLine("Home >> Login >> My Account >> Transfer Money");
        Console.Write("Enter recipient's IBAN: ");
        if (!int.TryParse(Console.ReadLine(), out int recipientIBAN))
        {
            Console.WriteLine("Invalid IBAN.");
            Console.ReadKey();
            PrintUserInterface(sender);
            return;
        }

        Console.Write("Enter amount to transfer: ");
        if (!double.TryParse(Console.ReadLine(), out double amount) || amount <= 0 || sender.balance < amount)
        {
            Console.WriteLine("Invalid amount or insufficient funds.");
            Console.ReadKey();
            PrintUserInterface(sender);
            return;
        }

        if (BankSystem.PerformTransaction(sender.IBAN, recipientIBAN, amount))
        {
            Console.WriteLine("Transfer was successful!");
        }
        else
        {
            Console.WriteLine("Transfer was not successful! ");
        }
            Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        PrintUserInterface(sender);
    }
    public static void PrintLoginScreen()
    {
        Console.Clear();
        Console.WriteLine("Home >> Login");
        Console.Write("Enter your IBAN: ");
        int iban;
        if (!int.TryParse(Console.ReadLine(), out iban))
        {
            Console.WriteLine("Invalid IBAN. Press any key to try again...");
            Console.ReadKey();
            PrintLoginScreen();
            return;
        }

        Console.Write("Enter your password: ");
        string? password = Console.ReadLine();

        Account? account = BankSystem.Authenticate(iban, password);
        Console.Clear();
        Console.WriteLine("Home >> Login");
        if (account != null)
        {
            Console.WriteLine("Login successful! Press any key to continue...");
            Console.ReadKey();
            PrintUserInterface(account);
        }
        else
        {
            Console.WriteLine("Invalid credentials. Press any key to try again...");
            Console.ReadKey();
            PrintLoginScreen();
        }
    }


    public static void PrintWelcomeScreen()
    {
        Console.Clear();
        Console.WriteLine("Home");
        Console.WriteLine("Hello and Welcome to Nolds international Bank System.");
        Console.WriteLine("Enter your prefered action");
        Console.WriteLine("1. Create New Account");
        Console.WriteLine("2. Create New Bank");
        Console.WriteLine("3. Login into Account");

        string? option = Console.ReadLine();
        switch (option)
        {
            case "1":
                PrintCreateNewAccount();
                break;

            case "2":
                PrintCreateNewBank();
                break;

            case "3":
                PrintLoginScreen();
                break;

            default:
                PrintWelcomeScreen();
                break;
        }
    }
    public static void PrintCreateNewAccount()
    {
        Console.Clear();
        Console.WriteLine("Home >> Create Account");
        Console.WriteLine("Enter your prefered action");
        Console.WriteLine("1. Create Account");
        Console.WriteLine("2. Go Back to Home");

        string? option = Console.ReadLine();
        switch (option)
        {
            case "1":
                Console.Clear();
                Console.WriteLine("Home >> Create Account");
                if (!BankSystem.AreBanksAvailable())
                {
                    Console.WriteLine("Sorry there arent any Banks available... please create a bank");
                    Console.WriteLine("Press any Key to continue...");
                    Console.ReadKey();
                    PrintWelcomeScreen();
                }
                
                Console.WriteLine("Enter the BIC of the Bank you want a account at:");
                byte chosenBIC = Convert.ToByte(Console.ReadLine());
                if (BankSystem.IsBICTaken(chosenBIC))
                {
                    Console.Clear();
                    Console.WriteLine("Home >> Create Account");
                    Console.WriteLine("What is the Name of the Owner of the account");
                    string? chosenName = Console.ReadLine();
                    Console.Clear();
                    Console.WriteLine("Home >> Create Account");
                    Console.WriteLine("Choose a Password for the account");
                    string? chosenPassword = Console.ReadLine();
                    Console.Clear();
                    Console.WriteLine("Home >> Create Account");
                    Console.WriteLine("Confirm your Details");
                    Console.WriteLine("BIC:" + chosenBIC);
                    Console.WriteLine("Owner Name: " + chosenName);
                    Console.WriteLine("Password: " + chosenPassword);
                    Console.WriteLine();
                    Console.WriteLine("Enter your prefered action");
                    Console.WriteLine("1. Confirm Account");
                    Console.WriteLine("2. Go Back to Home");
                    string? option2 = Console.ReadLine();
                    if (option2 == "1")
                    {
                        Bank? choosenBank = BankSystem.GetBankByBIC(chosenBIC);
                        Account? newAcc = null;
                        if(choosenBank != null)
                        {
                            newAcc = choosenBank.CreateAccount(chosenName, chosenPassword);
                        }
                        
                        Console.Clear();
                        Console.WriteLine("Home >> Create Account");
                        Console.WriteLine("Your Account has been created");
                        Console.WriteLine("Your IBAN is: " + newAcc?.IBAN);
                        Console.WriteLine("Press any Key to continue...");
                        Console.ReadKey();
                        PrintWelcomeScreen();
                    }
                    else
                    {
                        PrintWelcomeScreen();
                    }
                }
                else
                {
                    Console.WriteLine("Sorry this bank does not exist. You can create it tho :)");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    PrintWelcomeScreen();
                }
                break;

            default:
                PrintWelcomeScreen(); break;

        }
    }
    public static void PrintCreateNewBank()
    {
        Console.Clear();
        Console.WriteLine("Home >> Create New Bank");
        Console.WriteLine("Enter your prefered action");
        Console.WriteLine("1. Create Bank");
        Console.WriteLine("2. Go Back to Home");

        string? option = Console.ReadLine();
        switch (option)
        {
            case "1":
                Console.Clear();
                Console.WriteLine("Home >> Create New Bank");
                Console.WriteLine("Enter a BIC for the new Bank (0-255)");
                byte newBIC = Convert.ToByte(Console.ReadLine());
                if (BankSystem.IsBICTaken(newBIC))
                {
                    Console.WriteLine("This BIC is Taken");
                    Console.WriteLine("Press any Key to try again...");
                    Console.ReadKey();
                    PrintCreateNewBank();
                }
                else
                {
                    BankSystem.CreateBank(newBIC);
                    Console.WriteLine("A new Bank with the BIC: " + newBIC);
                    Console.WriteLine("Has been created");
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                    PrintWelcomeScreen();
                }
                break;


            case "2":
                PrintWelcomeScreen();
                break;



            default:
                PrintWelcomeScreen();
                break;
        }

    }
}
public static class BankSystem
{
    public static Dictionary<byte, Bank> banks = new Dictionary<byte, Bank>();

    public static Bank? CreateBank(byte bic)
    {
        Bank? bank = new Bank(bic);
        banks.Add(bic, bank);
        return banks.GetValueOrDefault(bic);
    }
    public static Bank? GetBankByBIC(byte bic)
    {
        return banks.GetValueOrDefault(bic);
    }
    public static bool IsBICTaken(byte BIC)
    {
        if (banks.ContainsKey(BIC))
        {
            return true;
        }
        return false;
    }
    public static bool AreBanksAvailable()
    {
        bool isAvailable = false;
        foreach (Bank bank in banks.Values)
        {
            if (bank.GetAvailableAccounts() > 0)
            {
                isAvailable = true;
                break;
            }
        }
        return isAvailable;
    }
    public static Account? Authenticate(int iban, string? password)
    {
        Bank? bank = GetBankByBIC(BankUtils.GetBICFromIBAN(iban));
        if (bank == null) return null;

        Account? account = bank.GetAccountById(BankUtils.GetIDFromIBAN(iban));
        if (password != null && account != null && account.password == password)
        {
            return account;
        }
        return null;
    }
    public static bool PerformTransaction(int senderIBAN, int receiverIBAN, double amount)
    {
        if (senderIBAN == receiverIBAN)
        {
            return false;
        }

        Bank? senderBank = banks.GetValueOrDefault(BankUtils.GetBICFromIBAN(senderIBAN));
        Bank? receiverBank = banks.GetValueOrDefault(BankUtils.GetBICFromIBAN(receiverIBAN));

        byte senderID = BankUtils.GetIDFromIBAN(senderIBAN);
        byte receiverID = BankUtils.GetIDFromIBAN(receiverIBAN);

        Debug.Assert(senderBank != null);
        Debug.Assert(receiverBank != null);

        Account? senderAccount = senderBank.GetAccountById(senderID);
        Account? receiverAccount = receiverBank.GetAccountById(receiverID);

        Debug.Assert(senderAccount != null);
        Debug.Assert(receiverAccount != null);

        if (senderAccount.balance >= amount)
        {
            senderAccount.balance -= amount;
            receiverAccount.balance += amount;
            return true;
        }
        return false;
    }
}
public static class BankUtils
{
    public static byte GetBICFromIBAN(int IBAN)
    {
        return SplitIBAN(IBAN).Item1;
    }
    public static int CombineToIBAN(byte BIC, byte accountID)
    {
        int combined = BIC << 8 | accountID;
        return combined;
    }
    public static byte GetIDFromIBAN(int IBAN)
    {
        return SplitIBAN(IBAN).Item2;
    }
    private static (byte, byte) SplitIBAN(int combined)
    {
        byte b1 = (byte)(combined >> 8); // Extract the upper byte
        byte b2 = (byte)(combined & 0xFF); // Extract the lower byte
        return (b1, b2);
    }
}
public class Bank
{
    public Bank(byte _BIC)
    {
        BIC = _BIC;
        accounts = new Dictionary<Byte, Account>();
    }

    public byte BIC; //Number 0-255
    Dictionary<Byte, Account> accounts;

    public byte GetAvailableAccounts()
    {
        return (byte)(255 - accounts.Count());
    }
    public Account? CreateAccount(string? ownerName, string? password)
    {
        if(ownerName == null || password == null)
        {
            return null;
        }
        Account newAccount = new Account(ownerName, password);
        newAccount.id = (byte)(GetHighestID() + 1);
        int IBAN = BankUtils.CombineToIBAN(BIC, newAccount.id);
        newAccount.IBAN = IBAN;
        accounts.Add(newAccount.id, newAccount);
        return accounts.GetValueOrDefault(newAccount.id);
    }
    public Account? GetAccountById(byte id)
    {
        return accounts.GetValueOrDefault(id);
    }

    private byte GetHighestID()
    {
        byte currentID = 0;
        foreach (byte account in accounts.Keys)
        {
            if (account > currentID)
            {
                currentID = account;
            }
        }
        return currentID;
    }
}
public class Account
{
    public Account(string _ownerName, string _password)
    {
        ownerName = _ownerName;
        password = _password;
        balance = 100;
    }

    public byte id;
    public string ownerName;
    public string password;
    public int IBAN;
    public double balance; //in €

}
