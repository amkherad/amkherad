using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RemoteProject.Shared.AspNet.Jobs;

public abstract class BackgroundJobBase : IHostedService, IDisposable
{
    private int _executionCount;
    private Timer? _timer;
    private CancellationToken? _cancellationToken;

    private readonly ILogger _logger;

    private TimeSpan _updateInterval;
    private readonly string _logName;

    protected BackgroundJobBase(
        ILogger logger,
        TimeSpan updateInterval
    )
    {
        _logger = logger;

        _logName = GetType().FullName!;

        _updateInterval = updateInterval;
    }

    protected BackgroundJobBase(
        ILogger<BackgroundJobBase> logger,
        TimeSpan updateInterval,
        string logName
    )
    {
        _logger = logger;

        _logName = logName;

        _updateInterval = updateInterval;
    }

    protected TimeSpan UpdateInterval
    {
        get => _updateInterval;
        set
        {
            if (_timer is null)
            {
                _timer = new Timer(DoWork, null, TimeSpan.Zero, _updateInterval);
            }
            else
            {
                _timer?.Change(TimeSpan.Zero, value);
            }
            _updateInterval = value;
        }
    }

    protected abstract Task Execute(int iteration, CancellationToken cancellationToken);

    private async void DoWork(object? state)
    {
        var iterationCount = Interlocked.Increment(ref _executionCount);

        await Execute(iterationCount, _cancellationToken ?? CancellationToken.None);
    }


    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        _logger.LogInformation($"{_logName} running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero, _updateInterval);

        return Task.CompletedTask;
    }

    public virtual Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{_logName} is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public virtual void Dispose()
    {
        _timer?.Dispose();
        _timer = null;
    }
}
