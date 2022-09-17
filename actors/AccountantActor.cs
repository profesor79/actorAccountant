using Akka.Actor;

namespace actors;
public class AccountantActor:ReceiveActor
{

    Dictionary<Guid, DelayedtTrasferNoLockMessage> _delayedtTrasferNoLock = new Dictionary<Guid, DelayedtTrasferNoLockMessage>();
	decimal _balance = 0;
	private readonly IActorRef _schedulerActor;

	

	public AccountantActor(IActorRef schedulerActor)
	{
        _schedulerActor = schedulerActor;

        Receive<GetBalanceMessage>(SendCurrentBalance);
		Receive<IncomingTransferMessage>(AddToBalance);
		Receive<WithdrawAmountMessage>(RemoveFromBalance);
		Receive<DelayedtTrasferNoLockMessage>(ScheduleDelayedNoLock);
		Receive<ScheduledNoLockTransferToExecuteMessage>(ExecuteScheduledNoLock);
		
	}

	private void ExecuteScheduledNoLock(ScheduledNoLockTransferToExecuteMessage m)
	{
		if (_delayedtTrasferNoLock.ContainsKey(m.TransferId)){
			var transferDetails = _delayedtTrasferNoLock[m.TransferId];
            
			//removind transfer details
			_delayedtTrasferNoLock.Remove(m.TransferId);

            // withdraw
            if (_balance - transferDetails.Amount < 0)
            {
                Sender.Tell(new NoAvaliableBalanceOnAcconutMessage());
                return;
            }

            _balance -= transferDetails.Amount;		
        }        
    }

	private void ScheduleDelayedNoLock(DelayedtTrasferNoLockMessage m)
	{
		_delayedtTrasferNoLock.Add(m.TransferId,  m);
		_schedulerActor.Tell(m);
		Sender.Tell(new DelayedtTrasferNoLockScheduledMessage());
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
		Sender.Tell(new CurrentBalanceMessage(_balance));	
	}

	public class GetBalanceMessage
	{
		
	}

	public class CurrentBalanceMessage
	{
		public decimal Balance { get; set; }

		public CurrentBalanceMessage(decimal balance)
		{
			Balance = balance;
		}
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

	public class DelayedtTrasferNoLockMessage
	{
		
		public DelayedtTrasferNoLockMessage(decimal amount, TimeSpan timeSpan, Guid transferId)
		{
			Amount = amount;
			TimeSpan = timeSpan;
			TransferId = transferId;
		}

		public decimal Amount { get; }
		public TimeSpan TimeSpan { get; }
		public Guid TransferId { get; }		
	}

	public class DelayedtTrasferNoLockScheduledMessage
	{
	}

	public class ScheduledNoLockTransferToExecuteMessage
	{

		public ScheduledNoLockTransferToExecuteMessage(Guid transferId)
		{
			TransferId = transferId;
		}

		public Guid TransferId { get; }
	}
}
