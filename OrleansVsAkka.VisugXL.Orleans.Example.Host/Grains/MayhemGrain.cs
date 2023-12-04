using OrleansVsAkka.VisugXL.Orleans.Example.Common;

namespace OrleansVsAkka.VisugXL.Orleans.Example.Host.Grains;

public class MayhemGrain : Grain, IMayhemGrain
{
    private IDisposable? _timer;

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        _timer?.Dispose();

        return base.OnDeactivateAsync(reason, cancellationToken);
    }

    public Task Start()
    {
        _timer = RegisterTimer(OnTimer, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        return Task.CompletedTask;
    }

    private async Task OnTimer(object arg)
    {
        for (int i = 0; i < 10000; i++)
        {
            var randomGrain = GrainFactory.GetGrain<IRandomGrain>(Guid.NewGuid());
            await randomGrain.GetRandomNumberAsync();
        }
    }
}