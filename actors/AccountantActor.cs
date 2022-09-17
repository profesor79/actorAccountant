using Akka.Actor;

namespace actors;
public class AccountantActor:ReceiveActor
{

	decimal _balance = 0;
	public AccountantActor()
	{
		Receive<GetBalanceMessage>(SendCurrentBalance);
	}

	private void SendCurrentBalance(GetBalanceMessage m)
	{
		Console.WriteLine($"sending balance: {_balance}");
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
}
