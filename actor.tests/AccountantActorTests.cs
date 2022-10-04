using actors;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using static actors.AccountantActor;

namespace actor.tests;

public class AccountantActorTests : ActorTestBase
{

    [Fact]
    public void T001GetZeroBalance()
    {
        //given accountant actor
        var props = Props.Create(() => new AccountantActor());
        _sut = Sys.ActorOf(props);

        // when we ask for a balance        
        _sut.Tell(new GetBalanceMessage());

        //then balance message shall be received
        ExpectMsg<CurrentBalanceMessage>(m =>
        {
            Assert.Equal(0, m.Balance);
        });
    }


    [Fact]
    public void T002HandleIncomingTransfer()
    {
        // objecitve: when we send a 100 then we shall get a balance plus a 100
        decimal amount = 100;
        // given an actor with zero balance
        T001GetZeroBalance();

        // when we send a 100 and ask for a balance
        _sut.Tell(new IncomingTransferMessage(amount));
        _sut.Tell(new GetBalanceMessage());

        // then balance shall be given
        ExpectMsg<CurrentBalanceMessage>(m =>
        {
            Assert.Equal(amount, m.Balance);
        });
    }


    [Fact]
    public void T003HandleOutgoingTransfer()
    {
        // objecitve: when we send a 100 then and request a 50 , we shall have 50
        decimal amount = 50;

        // given an actor with given balance
        T002HandleIncomingTransfer();

        // when we ask for a withdrawal, then accout balance shall be deducted
        _sut.Tell(new WithdrawAmountMessage(amount));
        _sut.Tell(new GetBalanceMessage());

        // then balance shall be given
        ExpectMsg<CurrentBalanceMessage>(m =>
        {
            Assert.Equal(amount, m.Balance);
        });

    }


    [Fact]
    public void T004PreventFromGoingInDebt()
    {
        // objecitve: when we send a 100 then and request of 150 shall not proceed
        decimal amount = 50;

        // given an actor with given balance of 100
        T002HandleIncomingTransfer();

        // when we ask for a withdrawal, then accout balance shall be deducted
        _sut.Tell(new WithdrawAmountMessage(3 * amount));


        // then we shall receive no Avaliable Balace and balence shall be 100
        ExpectMsg<NoAvaliableBalanceOnAcconutMessage>();
        CheckBalance(2 * amount);
    }

    [Fact]
    public void T005LockAmountOnAccount()
    {
        // given account with a 100 on it
        T002HandleIncomingTransfer();

        //when we send lock message 
        decimal amount = 50;
        _sut.Tell(new LockAmountMessage(amount));
        // then balance shall be same, but avaliable funds less
        CheckBalance(50, 100);

    }



    [Fact]
    public void T006UnLockAmountOnAccount()
    {
        // given account with a 100 on it and 50 locked
        T005LockAmountOnAccount();

        //when we send lock message 
        decimal amount = 30;
        _sut.Tell(new UnLockAmountMessage(amount));
        
        // then balance shall be same, but avaliable funds less
        CheckBalance(80, 100);

    }

    private void CheckBalance(decimal avaliable, decimal balance)
    {
        // check balance
        _sut.Tell(new GetBalanceMessage());

        ExpectMsg<CurrentBalanceMessage>(m =>
        {
            Assert.Equal(balance, m.Balance);
            Assert.Equal(avaliable, m.Avaliable);
        });
    }

    private void CheckBalance(decimal amount)
    {

        // check balance
        _sut.Tell(new GetBalanceMessage());

        ExpectMsg<CurrentBalanceMessage>(m =>
        {
            Assert.Equal(amount, m.Balance);
        });
    }

    
}