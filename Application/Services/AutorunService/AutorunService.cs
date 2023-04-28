namespace Application.Services.AutorunService;

public class AutorunService : BackgroundService
{
    private const int _oneMinuteInMiliseconds = 60000;
    private const int _twoMinutes = 2;
    private const int _tenMinutes = 10;

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ITokenBlockingService _tokenBlockingService;
    private readonly ILogger<AutorunService> _logger;

    private readonly (DateTime, Func<Task>, int)[] ExecutionQueue;

    private static ulong _counter = 0;

    public AutorunService(IServiceScopeFactory serviceScopeFactory, ITokenBlockingService tokenBlockingService, ILogger<AutorunService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _tokenBlockingService = tokenBlockingService;
        _logger = logger;

        ExecutionQueue = new (DateTime, Func<Task>, int)[]
        {
            (DateTime.UtcNow, ChangeDailyRandom, _tenMinutes),
            (DateTime.UtcNow, CleanBannedTokens, _twoMinutes)
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            bool nothingToDo = true;
            _logger.LogInformation($"Autorun [{_counter}] is working.");

            for (int i = 0; i < ExecutionQueue.Length; i++) 
            {
                if (ExecutionQueue[i].Item1.AddMinutes(ExecutionQueue[i].Item3) <= DateTime.UtcNow)
                {
                    nothingToDo = false;
                    await ExecutionQueue[i].Item2();
                    ExecutionQueue[i].Item1 = DateTime.UtcNow;
                }
            }

            if(nothingToDo) 
                _logger.LogInformation($"Autorun [{_counter}] has nothing to do, skipping.");
            else
                _logger.LogInformation($"Autorun [{_counter}] finished.");

            _counter++;

            await Task.Delay(_oneMinuteInMiliseconds);
        }
    }

    private async Task ChangeDailyRandom()
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IDailyRandomService>();
            _logger.LogInformation($" - Autorun is running: {nameof(scopedProcessingService)}.");
            await scopedProcessingService.ChangeDailyRandom();
        }
    }

    private async Task CleanBannedTokens()
    {
        _logger.LogInformation($" - Autorun is running: {nameof(_tokenBlockingService)}.");
        await _tokenBlockingService.RemoveExpiredTokensData();
    }
}