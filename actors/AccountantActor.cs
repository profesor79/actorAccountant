using Akka.Actor;

namespace actors;
public class AccountantActor: ReceiveActor
{
    decimal _balance = 0;
    decimal _lockedAmount = 0;





    public AccountantActor( )
    {
        Receive<GetBalanceMessage>(SendCurrentBalance);
        Receive<IncomingTransferMessage>(AddToBalance);
        Receive<WithdrawAmountMessage>(RemoveFromBalance);
        Receive<LockAmountMessage>(LockAmount);
        Receive<UnLockAmountMessage>(UnlockAmount);
    }

    private void UnlockAmount(UnLockAmountMessage m)
    {
        _lockedAmount -= m.Amount;
    }

    private void LockAmount(LockAmountMessage m)
    {
        _lockedAmount += m.Amount;
    }

    private void RemoveFromBalance(WithdrawAmountMessage m)
    {
        if (_balance - m.Amount < 0) {
            Sender.Tell(new NoAvaliableBalanceOnAcconutMessage());
            return;
        }       

        _balance -= m.Amount;
    }

    private void AddToBalance(IncomingTransferMessage m)
    {
        _balance += m.Amount;
    }

    private void SendCurrentBalance(GetBalanceMessage m)
    {
        Sender.Tell(new CurrentBalanceMessage(_balance, _balance-_lockedAmount));	
    }

    public class GetBalanceMessage
    {
        
    }

    public class CurrentBalanceMessage
    {
        public decimal Balance { get; }
        public decimal Avaliable { get; }

        public CurrentBalanceMessage(decimal balance, decimal avaliable)
        {
            Balance = balance;
            Avaliable = avaliable;
        }
    }


    public class UnLockAmountMessage
    {        

        public UnLockAmountMessage(decimal amount)
        {
            Amount = amount;
        }

        public decimal Amount { get; }
    }
    public class IncomingTransferMessage
    {
        

        public IncomingTransferMessage(decimal amount)
        {
            Amount = amount;
        }

        public decimal Amount { get; }
    }

    public class WithdrawAmountMessage
    {

        public WithdrawAmountMessage(decimal amount)
        {
            Amount = amount;
        }

        public decimal Amount { get; }
    }

    public class NoAvaliableBalanceOnAcconutMessage
    {
    }

    public class LockAmountMessage
    {
        

        public LockAmountMessage(decimal amount)
        {
            Amount = amount;
        }

        public decimal Amount { get; }
    }
}
