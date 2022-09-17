using actors;
using Akka.Actor;
using Akka.TestKit.Xunit2;

namespace actor.tests;

public class AccountantActorTests:ActorTestBase
{
    
    [Fact]
    public void T001GetZeroBalance()
    {
        //given accountant actor
        var props = Props.Create(() => new AccountantActor());
        _sut = Sys.ActorOf(props);

        // when we ask for a balance        
        _sut.Tell(new AccountantActor.GetBalanceMessage());

        //then balance message shall be received
        ExpectMsg<AccountantActor.CurrentBalanceMessage>(m => {
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
        _sut.Tell(new AccountantActor.IncomingTransferMessage(amount));
        _sut.Tell(new AccountantActor.GetBalanceMessage());

        // then balance shall be given
        ExpectMsg<AccountantActor.CurrentBalanceMessage>(m => {
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
        _sut.Tell(new AccountantActor.WithdrawAmountMessage(amount));
        _sut.Tell(new AccountantActor.GetBalanceMessage());

        // then balance shall be given
        ExpectMsg<AccountantActor.CurrentBalanceMessage>(m => {
            Assert.Equal(amount, m.Balance);
        });

    }


    [Fact]
    public void T004PreventFromGoingInDebt()
    {
        // objecitve: when we send a 100 then and request a 150 no we shall not proceed
        decimal amount = 50;

        // given an actor with given balance
        T002HandleIncomingTransfer();

        // when we ask for a withdrawal, then accout balance shall be deducted
        _sut.Tell(new AccountantActor.WithdrawAmountMessage(3*amount));


        // then we shall receive no Avaliable Balace and balence shall be 100
        ExpectMsg<AccountantActor.NoAvaliableBalanceOnAcconutMessage>();
        
        _sut.Tell(new AccountantActor.GetBalanceMessage());
               
        ExpectMsg<AccountantActor.CurrentBalanceMessage>(m => {
            Assert.Equal(2*amount, m.Balance);
        });
    }

}