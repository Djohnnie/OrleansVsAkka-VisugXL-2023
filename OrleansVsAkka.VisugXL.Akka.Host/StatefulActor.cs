using Akka.Actor;

namespace OrleansVsAkka.VisugXL.Akka.Host;

public class State
{
    public List<string> Items { get; } = new ();
}

public record AddItemMessage(string Item);

public class StatefulActor : ReceiveActor
{
    private State _state = new();

    public StatefulActor()
    {
        Receive<AddItemMessage>(HandleAddItem);
    }

    private void HandleAddItem(AddItemMessage message)
    {
        _state.Items.Add(message.Item);
    }
}