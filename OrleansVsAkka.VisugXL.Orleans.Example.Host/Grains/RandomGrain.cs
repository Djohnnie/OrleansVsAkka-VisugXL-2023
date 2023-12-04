using OrleansVsAkka.VisugXL.Orleans.Example.Common;

namespace OrleansVsAkka.VisugXL.Orleans.Example.Host.Grains;

public class RandomGrain : Grain, IRandomGrain
{
    public Task<int> GetRandomNumberAsync()
    {
        var randomNumber = Random.Shared.Next();
        return Task.FromResult(randomNumber);
    }
}