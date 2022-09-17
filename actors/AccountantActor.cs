using Akka.Actor;

namespace actors;
public class AccountantActor:ReceiveActor
{

	List<DelayedtTrasferNoLockMessage> _delayedtTrasferNoLockMessages = new List<DelayedtTrasferNoLockMessage>();
	decimal _balance = 0;
	private readonly IActorRef _schedulerActor;

	public AccountantActor(IActorRef schedulerActor)
	{
		Receive<GetBalanceMessage>(SendCurrentBalance);
		Receive<IncomingTransferMessage>(AddToBalance);
		Receive<WithdrawAmountMessage>(RemoveFromBalance);
		Receive<DelayedtTrasferNoLockMessage>(ScheduleDelayedNoLock);
		_schedulerActor = schedulerActor;
	}

	private void ScheduleDelayedNoLock(DelayedtTrasferNoLockMessage m)
	{
		_delayedtTrasferNoLockMessages.Add(m);
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
		
		public DelayedtTrasferNoLockMessage(decimal amount, TimeSpan timeSpan)
		{
			Amount = amount;
			TimeSpan = timeSpan;
		}

		public decimal Amount { get; }
		public TimeSpan TimeSpan { get; }
		public Guid Id { get; set; } = Guid.NewGuid();	
	}

	public class DelayedtTrasferNoLockScheduledMessage
	{
	}
}
