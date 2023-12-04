using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Hosting;
using OrleansVsAkka.VisugXL.Akka.Example.Host.Messages;
using OrleansVsAkka.VisugXL.Akka.Example.Host.Sharding;

namespace OrleansVsAkka.VisugXL.Akka.Example.Host.Actors;

public class MayhemActor: ReceiveActor
{
    private readonly IActorRef _region;

    public MayhemActor()
    {
        Receive<StartMayhem>(HandleStartMayhem);
        Receive<CreateRandomActor>(HandleCreateRandomActor);

        var resolver = DependencyResolver.For(Context.System);
        var registry = resolver.Resolver.GetService<ActorRegistry>();
        _region = registry.Get<RegistryKey.RandomNumberRegion>();
    }

    private void HandleStartMayhem(StartMayhem message)
    {
        Context.System.Scheduler.ScheduleTellRepeatedly(
            initialDelay: TimeSpan.Zero, 
            interval: TimeSpan.FromMilliseconds(100),
            receiver: Self,
            message: new CreateRandomActor(),
            sender: Self);
    }

    private void HandleCreateRandomActor(CreateRandomActor obj)
    {
        _region.Tell(new GenerateRandomNumber(Guid.NewGuid()));
    }

    public static Props CreateProps()
    {
        return Props.Create<MayhemActor>();
    }
}

public class RandomNumberGeneratorActor : ReceiveActor
{
    private readonly Guid _identifier;

    public RandomNumberGeneratorActor(Guid identifier)
    {
        _identifier = identifier;
        Receive<GenerateRandomNumber>(HandleGenerateRandomNumber);
    }

    private void HandleGenerateRandomNumber(GenerateRandomNumber obj)
    {
        var random = new Random();
        var number = random.Next(1000);
        Console.WriteLine($"Random number generated at {_identifier}: {number}");
    }

    public static Props CreateProps(Guid identifier)
    {
        return Props.Create<RandomNumberGeneratorActor>(identifier);
    }
}