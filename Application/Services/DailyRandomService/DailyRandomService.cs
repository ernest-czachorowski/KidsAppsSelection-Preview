namespace Application.Services.DailyRandomService;

public class DailyRandomService : IDailyRandomService
{
    private readonly DataContext _context;
    private readonly ILogger<DailyRandomService> _logger;

    public DailyRandomService(DataContext context, ILogger<DailyRandomService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ChangeDailyRandom()
    {
        if (_context.Apps.Any())
        {
            _logger.LogInformation("Changing daily random now.");

            Random random = new();

            await _context.Apps.ForEachAsync(a => a.DailyRandom = random.Next());

            if (await _context.SaveChangesAsync() == 0)
            {
                _logger.LogError("Can't change daily random. Error occured while saving changes to database.");
                return;
            }

            _logger.LogInformation("Daily random changed.");
        }
        else
        {
            _logger.LogInformation("Trying to change daily random, but no applications were found in database. Skipping now.");
        }
    }
}
