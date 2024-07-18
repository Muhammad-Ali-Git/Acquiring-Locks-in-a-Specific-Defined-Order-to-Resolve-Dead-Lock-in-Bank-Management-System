using System;
using System.Diagnostics;
using System.Threading;

class Ali
{
    public static void Main()
    {
        Stopwatch SW = Stopwatch.StartNew();

        Console.WriteLine("This program manages bank account of a user.");
        Console.WriteLine("---------------------------------------------------------------------------------------------------------");

        Console.WriteLine("Main Started!");
        Console.WriteLine("---------------------------------------------------------------------------------------------------------");

        Account A = new Account (101, 5000);
        Account B = new Account (102, 3000);
        
        AccountManager AMA = new AccountManager (A, B, 1000, SW);
        Thread T1 = new Thread (AMA.Transfer);
        T1.Name = "T1";

        AccountManager AMB = new AccountManager (B, A, 2000, SW);
        Thread T2 = new Thread(AMB.Transfer);
        T2.Name = "T2";

        T1.Start();
        T2.Start();

        T1.Join();
        T2.Join();

        SW.Stop();

        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------");
        Console.WriteLine("Main Ended!");

        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------");
        Console.WriteLine("Total elapsed time: {0} ms", SW.ElapsedMilliseconds);
    }
}

public class Account
{
    double _balance;
    int _id;

    public Account (int id, double balance)
    {
        this._id = id;
        this._balance = balance;
    }

    public int id
    {
        get
        {
            return _id;
        }
    }

    public void Withdraw (double amount)
    {
        _balance = _balance - amount;
    }

    public void Deposit (double amount)
    {
        _balance = _balance + amount;
    }
}

public class AccountManager
{
    Account _fromAccount;
    Account _toAccount;
    double _TransferAmount;
    Stopwatch _stopwatch;
    
    public AccountManager(Account fromAccount, Account toAccount, double TransferAmount, Stopwatch stopwatch)
    {
        this._fromAccount = fromAccount;
        this._toAccount = toAccount;
        this._TransferAmount = TransferAmount;
        this._stopwatch = stopwatch;
    }

    public void Transfer()
    {
        object _lock1, _lock2;

        if(_fromAccount.id < _toAccount.id)
        {
            _lock1 = _fromAccount;
            _lock2 = _toAccount;
        }
        else
        {
            _lock1 = _toAccount;
            _lock2 = _fromAccount;
        }
        _stopwatch.Stop();

        Console.WriteLine("{0} trying to acquire lock on {1}", Thread.CurrentThread.Name, ((Account)_lock1).id.ToString());
            
        lock(_lock1)
        {
            Console.WriteLine("{0} acquired lock on {1}", Thread.CurrentThread.Name, ((Account)_lock1).id.ToString());
            Console.WriteLine("{0} suspended for 1s.", Thread.CurrentThread.Name);

            Thread.Sleep(1000);

            Console.WriteLine("{0} back in action and trying to acquire lock on {1}", Thread.CurrentThread.Name, ((Account)_lock2).id.ToString());
            lock(_lock2)
            {
                Console.WriteLine("{0} acquired lock on {1}", Thread.CurrentThread.Name, ((Account)_lock2).id.ToString());

                _fromAccount.Withdraw(_TransferAmount);
                _toAccount.Deposit(_TransferAmount);

                Console.WriteLine("{0} transferred {1} from {2} to {3}", Thread.CurrentThread.Name, _TransferAmount.ToString(), _fromAccount.id.ToString(), _toAccount.id.ToString());
            }
        }
    }
}