namespace Application.Services.AppService;

public class AppService : IAppService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IReceivedTokenService _receivedTokenService;
    private readonly IAppFromUrlService _appFromUrlService;

    public AppService(DataContext context, IMapper mapper, IReceivedTokenService receivedTokenService, IAppFromUrlService appFromUrlService)
    {
        _context = context;
        _mapper = mapper;
        _receivedTokenService = receivedTokenService;
        _appFromUrlService = appFromUrlService;
    }

    public async Task<(HttpStatusCode, ServiceResponse<List<App_DTO>>)> LoadAllApps()
    {
        var dbResult = await _context.LoadAllAppsAsync();

        if (dbResult.Count <= 0)
            return HttpResponse<List<App_DTO>>.NotFound();

        return HttpResponse<List<App_DTO>>.OK(_mapper.Map<List<App_DTO>>(dbResult), "Complete list of apps.");
    }

    public async Task<(HttpStatusCode, ServiceResponse<App_DTO>)> LoadAppById(Guid request)
    {
        var dbResult = await _context.LoadAppByIdWithIncludesAsync(request);

        if (dbResult is null)
            return HttpResponse<App_DTO>.NotFound($"The requested app could not be found in the database.");

        return HttpResponse<App_DTO>.OK(_mapper.Map<App_DTO>(dbResult), $"App loaded: {dbResult.Title}.");
    }

    public async Task<(HttpStatusCode, ServiceResponse<App_DTO>)> UpdateApp(App_DTO request)
    {
        var dbResult = await _context.LoadAppByIdAsync(request.Id);

        if (dbResult is null)
            return HttpResponse<App_DTO>.NotFound($"The app {request.Title} was not found in the database.");

        _context.Entry(dbResult).State = EntityState.Modified;

        _mapper.Map(request, dbResult);
        dbResult.UpdateDate_UTC = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return HttpResponse<App_DTO>.OK(_mapper.Map<App_DTO>(dbResult), $"The app {request.Title} was successfully updated in the database.");
    }

    public async Task<(HttpStatusCode, ServiceResponse<bool>)> DeleteAppById(Guid request)
    {
        var dbResult = await _context.LoadAppByIdAsync(request);

        if (dbResult is null)
            return HttpResponse<bool>.NotFound($"The requested app could not be found in the database.");

        _context.Apps.Remove(dbResult);

        await _context.SaveChangesAsync();

        return HttpResponse<bool>.OK(true, $"The app {dbResult.Title} was successfully deleted from the database.");
    }

    public async Task<(HttpStatusCode, ServiceResponse<App_DTO>)> ValidateApp(App request)
    {
        if (string.IsNullOrEmpty(request.Title))
            return HttpResponse<App_DTO>.BadRequest("The app title is null or empty.");

        if (string.IsNullOrEmpty(request.Icon))
            return HttpResponse<App_DTO>.BadRequest("The app iscon is null or empty.");

        if (request.Images is null || !request.Images.Any())
            return HttpResponse<App_DTO>.BadRequest("The app images either do not exist or the server is unable to retrieve them.");

        var dbResult = await _context.IsAppWithUrlOrTittleAsync(request.Url, request.Title);

        if (dbResult)
            return HttpResponse<App_DTO>.Confilct("The app already exists in the database. However, it could be not visible to users.");

        return HttpResponse<App_DTO>.OK(_mapper.Map<App_DTO>(request), $"The app {request.Title} seems to be OK.");
    }

    public async Task<(HttpStatusCode, ServiceResponse<App_DTO>)> AddApp(AddApp_DTO request)
    {
        App app = await _appFromUrlService.GooglePlayStore(request.Url);

        app.Status = AppStatus.Hidden;
        app.DailyRandom = 0;

        var user = await _context.LoadUserByIdAsync(_receivedTokenService.IdFromToken());

        if (user is null)
            return HttpResponse<App_DTO>.Unauthorized("Your token is invalid.");

        app.AddedBy = user;

        (var statusCode, var result) = await ValidateApp(app);

        if(!statusCode.Equals(HttpStatusCode.OK))
            return (statusCode, result);

        _context.Entry(app).State = EntityState.Modified;
        _context.Entry(user).State = EntityState.Modified;

        user.Apps.Add(app);
        _context.Apps.Add(app);

        await _context.SaveChangesAsync();

        return HttpResponse<App_DTO>.OK(_mapper.Map<App_DTO>(app), $"{app.Title} has been successfully created.");
    }

    public async Task<(HttpStatusCode, ServiceResponse<List<App_DTO>>)> LoadManyApps(LoadManyApps_DTO request)
    {
        var dbResult = await _context.LoadManyAppsAsync(request.SearchedText, request.Status, request.Platform, request.Start, request.ItemsToLoad);

        if (!dbResult.Any())
        {
            if (request.Start == 0)
                return HttpResponse<List<App_DTO>>.NotFound();
            else
                return HttpResponse<List<App_DTO>>.OK(new List<App_DTO>(), "There is no more data to load.");
        }

        if (dbResult.Count < request.ItemsToLoad)
            return HttpResponse<List<App_DTO>>.OK(_mapper.Map<List<App_DTO>>(dbResult), "There is no more data to load.");
        else
            return HttpResponse<List<App_DTO>>.OK(_mapper.Map<List<App_DTO>>(dbResult), "The requested data has been successfully loaded.");
    }
}

