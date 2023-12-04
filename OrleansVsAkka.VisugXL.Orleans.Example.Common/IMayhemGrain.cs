namespace OrleansVsAkka.VisugXL.Orleans.Example.Common;

public interface IMayhemGrain : IGrainWithGuidKey
{
    Task Start();
}