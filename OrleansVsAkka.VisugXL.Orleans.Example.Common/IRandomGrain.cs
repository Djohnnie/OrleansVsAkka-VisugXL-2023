namespace OrleansVsAkka.VisugXL.Orleans.Example.Common;

public interface IRandomGrain : IGrainWithGuidKey
{
    Task<int> GetRandomNumberAsync();
}