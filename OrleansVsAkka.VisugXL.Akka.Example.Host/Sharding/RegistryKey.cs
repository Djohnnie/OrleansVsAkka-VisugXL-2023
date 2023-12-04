using Akka.Cluster.Sharding;
using OrleansVsAkka.VisugXL.Akka.Example.Host.Messages;

namespace OrleansVsAkka.VisugXL.Akka.Example.Host.Sharding;

public static class RegistryKey
{
    public record struct RandomNumberRegion;
}

public class GenerateRandomNumberExtractor : IMessageExtractor
{
    public string EntityId(object message)=> ((GenerateRandomNumber)message).Identifier.ToString();

    public object EntityMessage(object message) => message;

    public string ShardId(object message) => ((GenerateRandomNumber)message).Identifier.ToString().Substring(0,8);
}
