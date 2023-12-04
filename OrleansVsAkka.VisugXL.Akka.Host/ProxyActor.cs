using Akka.Actor;

namespace OrleansVsAkka.VisugXL.Akka.Host;

public record MessageToForward;
public record ConnectProxy;
public record ProxyConnected(IActorRef ActualActor);

public class ProxyActor : ReceiveActor, IWithUnboundedStash
{
    private IActorRef _remoteActor;

    public ProxyActor()
    {
        Become(Started);
    }

    private void Started()
    {
        Receive<MessageToForward>(_ => Stash.Stash());
        Receive<ProxyConnected>(HandleProxyConnected);
    }

    private void Initialized()
    {
        Receive<MessageToForward>(ForwardMessage);
    }

    protected override void PreStart()
    {
        var selection = new ActorSelection(Self,"akka.tcp://RemoteSystem@remotehost:5443/usr/target-actor");
        selection.Tell(new ConnectProxy());
    }

    private void HandleProxyConnected(ProxyConnected message)
    {
        _remoteActor = message.ActualActor;
        Become(Initialized);
        Stash.UnstashAll();
    }

    private void ForwardMessage(MessageToForward message)
    {
        _remoteActor?.Tell(message);
    }

    public IStash Stash { get; set; }
}

public class TargetActor : ReceiveActor
{
    public TargetActor()
    {
        Receive<ConnectProxy>(HandleConnectProxy);
        Receive<MessageToForward>(HandleActualMessage);
    }

    private void HandleConnectProxy(ConnectProxy message)
    {
        Sender.Tell(new ProxyConnected(Self));
    }

    private void HandleActualMessage(MessageToForward obj)
    {
        // Do something here
    }
}

       