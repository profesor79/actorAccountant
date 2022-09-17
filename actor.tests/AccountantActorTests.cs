using actors;
using Akka.Actor;
using Akka.TestKit.Xunit2;

namespace actor.tests;

public class AccountantActorTests:ActorTestBase
{
    
    [Fact]
    public void Test1()
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
}