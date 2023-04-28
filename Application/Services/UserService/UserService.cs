namespace Application.Services.UserService;

public class UserService : IUserService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IReceivedTokenService _receivedTokenService;
    private readonly ITokenBlockingService _tokenBlockingService;

    public UserService(DataContext context, IMapper mapper, IReceivedTokenService receivedTokenService, ITokenBlockingService tokenBlockingService)
    {
        _context = context;
        _mapper = mapper;
        _receivedTokenService = receivedTokenService;
        _tokenBlockingService = tokenBlockingService;
    }

    public async Task<(HttpStatusCode, ServiceResponse<User_DTO>)> GetMyProfile()
    {
        var dbResult = await _context.LoadUserByIdWithIncludesAsync(_receivedTokenService.IdFromToken());

        if (dbResult is null)
            return HttpResponse<User_DTO>.NotFound("The requested profile not found in database.");

        return HttpResponse<User_DTO>.OK(_mapper.Map<User_DTO>(dbResult), "The requested data has been successfully loaded.");
    }

    public async Task<(HttpStatusCode, ServiceResponse<User_DTO>)> LoadUserById(Guid request)
    {
        var dbResult = await _context.LoadUserByIdWithIncludesAsync(request);

        if (dbResult is null)
            return HttpResponse<User_DTO>.NotFound();

        return HttpResponse<User_DTO>.OK(_mapper.Map<User_DTO>(dbResult), $"User loaded: {dbResult.Username}.");
    }

    public async Task<(HttpStatusCode, ServiceResponse<User_DTO>)> LoadUserByUsername(string request)
    {
        var dbResult = await _context.LoadUserByUsernameWithIncludesAsync(request);

        if (dbResult is null)
            return HttpResponse<User_DTO>.NotFound();

        return HttpResponse<User_DTO>.OK(_mapper.Map<User_DTO>(dbResult), $"User loaded: {dbResult.Username}.");
    }

    public async Task<(HttpStatusCode, ServiceResponse<User_DTO>)> UpdateUser(User_DTO request)
    {
        var dbResult = await _context.LoadUserByIdAsync(request.Id);

        if (dbResult is null)
            return HttpResponse<User_DTO>.NotFound($"The requested user {request.Username} could not be found in the database.");

        _context.Entry(dbResult).State = EntityState.Modified;

        _mapper.Map(request, dbResult);
        dbResult.UpdateDate_UTC = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _tokenBlockingService.BlockAuthTokens(request.Id);

        return HttpResponse<User_DTO>.OK(_mapper.Map<User_DTO>(dbResult), $"The user {request.Username} was successfully updated in the database.");
    }

    public async Task<(HttpStatusCode, ServiceResponse<bool>)> DeleteUserById(Guid request)
    {
        var dbResult = await _context.LoadUserByIdAsync(request);

        if (dbResult is null)
            return HttpResponse<bool>.NotFound($"The requested user with id: {request} could not be found in the database.");

        _context.Users.Remove(dbResult);

        await _context.SaveChangesAsync();

        await _tokenBlockingService.BlockAuthTokens(request);

        return HttpResponse<bool>.OK(true, $"The user {dbResult.Username} was successfully deleted from the database.");
    }

    public async Task<(HttpStatusCode, ServiceResponse<List<User_DTO>>)> LoadManyUsers(LoadManyUsers_DTO request)
    {
        var dbResult = await _context.LoadManyUsersAsync(request.SearchedText, request.Role, request.Start, request.ItemsToLoad);

        if (!dbResult.Any())
        {
            if (request.Start == 0)
                return HttpResponse<List<User_DTO>>.NotFound();
            else
                return HttpResponse<List<User_DTO>>.OK(new List<User_DTO>(), "There is no more data to load.");
        }

        if (dbResult.Count < request.ItemsToLoad)
            return HttpResponse<List<User_DTO>>.OK(_mapper.Map<List<User_DTO>>(dbResult), "There is no more data to load.");
        else
            return HttpResponse<List<User_DTO>>.OK(_mapper.Map<List<User_DTO>>(dbResult), "The requested data has been successfully loaded.");
    }
}

