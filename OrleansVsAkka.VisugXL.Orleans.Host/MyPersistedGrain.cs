using Orleans.Runtime;
using System.Text.Json.Serialization;

namespace OrleansVsAkka.VisugXL.Orleans.Host;

public interface IMyPersistedGrain : IGrainWithGuidKey
{

}

public class MyPersistedGrain
{

    public async Task DoSomething()
    {
    }
}