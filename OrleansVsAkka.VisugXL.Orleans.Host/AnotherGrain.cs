namespace OrleansVsAkka.VisugXL.Orleans.Host;


public interface IAnotherGrain : IGrainWithGuidKey
{
    Task Something();
}

public class AnotherGrain : Grain, IAnotherGrain
{
    public Task Something()
    {
        return Task.CompletedTask;
    }
}