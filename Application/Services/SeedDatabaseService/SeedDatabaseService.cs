namespace Application.Services.SeedDatabaseService;

public class SeedDatabaseService
{
    private readonly DataContext _context;
    private readonly ILogger<SeedDatabaseService> _logger;
    private readonly IAuthService _authService;
    private readonly IAppFromUrlService _appFromUrlService;

    public SeedDatabaseService(DataContext context, ILogger<SeedDatabaseService> logger, IAuthService authService, IAppFromUrlService appFromUrlService)
    {
        _context = context;
        _logger = logger;
        _authService = authService;
        _appFromUrlService = appFromUrlService;
    }

    public async Task Seed()
    {
        if (!_context.Users.Any())
        {
            try
            {
                foreach (var userRoleDict in SeedData.UsersRolesDict)
                    await _authService.Register(userRoleDict.Key, userRoleDict.Value);
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception: {e.GetType()} from: {e.Source} message: {e.Message}");
            }
        }

        if (!_context.Apps.Any())
        {
            try
            {
                foreach (var app in SeedData.GooglePlayStoreData)
                    await SeedApps(new AddApp_DTO { Url = app });
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception: {e.GetType()} from: {e.Source} message: {e.Message}");
            }
        }
    }

    private async Task SeedApps(AddApp_DTO request)
    {
        Random random = new Random();

        App app = await _appFromUrlService.GooglePlayStore(request.Url);

        Array status = Enum.GetValues(typeof(AppStatus));
        app.Status = (AppStatus)status.GetValue(random.Next(status.Length))!;

        Array platform = Enum.GetValues(typeof(AppPlatform));
        app.Platform = (AppPlatform)platform.GetValue(random.Next(platform.Length))!;

        var user = await _context.Users.Skip(random.Next(0, _context.Users.Count())).Take(1).FirstAsync();
        app.AddedBy = user;

        app.AddDate_UTC = DateTime.UtcNow;
        app.UpdateDate_UTC = DateTime.UtcNow;
        app.DailyRandom = random.Next();

        if (string.IsNullOrEmpty(app.Title))
        {
            _logger.LogError($"The app title is null or empty. Request url: {request.Url}.");
            return;
        }

        if (string.IsNullOrEmpty(app.Icon))
        {
            _logger.LogError($"The app iscon is null or empty. Request url: {request.Url}.");
            return;
        }

        if (app.Images == null || app.Images.Length == 0)
        {
            _logger.LogError($"The app images either do not exist or the server is unable to retrieve them. Request url: {request.Url}.");
            return;
        }

        var exists = await _context.Apps.AnyAsync(a => a.Url == app.Url || a.Title == app.Title);

        if (exists)
        {
            _logger.LogError($"The app already exists in the database. Request url: {request.Url}.");
            return;
        }

        user.Apps.Add(app);

        _context.Users.Update(user);
        _context.Apps.Add(app);

        await _context.SaveChangesAsync();
    }
}

public static class SeedData
{
    public static string[] GooglePlayStoreData =
    {
        "https://play.google.com/store/apps/details?id=com.FunEduApp.FunEduFarm&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=me.pou.app&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.kitkagames.fallbuddies&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.ea.games.r3_row&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.mojang.minecraftpe&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.gameloft.android.ANMP.GloftICHM&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.marmalade.monopoly&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.sega.sonic1px&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.FDGEntertainment.redball4.gp&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.nintendo.zaka&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.rovio.baba&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.rovio.angrybirdsfriends&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.halfbrick.fruitninjafree&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.roblox.client&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.soccer.score.star&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.fugo.wow&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.hutchgames.formularacing&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.miniclip.footballstrike&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.hutchgames.cccg&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.joyjourney.PianoWhiteGo&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.turborilla.bike.racing.madskillsmotocross3&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.CarXTech.CarXDriftRacingFull&hl=en_US&gl=US",
        "https://play.google.com/store/apps/details?id=com.pixonic.wwr",
        "https://play.google.com/store/apps/details?id=com.supercell.brawlstars",
        "https://play.google.com/store/apps/details?id=com.tocaboca.tocalifeworld",
        "https://play.google.com/store/apps/details?id=com.wildlife.games.battle.royale.free.zooba",
        "https://play.google.com/store/apps/details?id=com.ea.game.simpsons4_row",
        "https://play.google.com/store/apps/details?id=com.sgs.emhq.android",
        "https://play.google.com/store/apps/details?id=com.pazugames.avatarworld",
        "https://play.google.com/store/apps/details?id=com.bethsoft.falloutshelter",
        "https://play.google.com/store/apps/details?id=com.minimuffin.kinderstories",
        "https://play.google.com/store/apps/details?id=com.starstable.horses",
        "https://play.google.com/store/apps/details?id=com.haugland.woa",
        "https://play.google.com/store/apps/details?id=com.ragequitgames.evillands",
        "https://play.google.com/store/apps/details?id=com.cobby.lonelysurvivor",
        "https://play.google.com/store/apps/details?id=com.ironhidegames.android.kingdomrush",
        "https://play.google.com/store/apps/details?id=com.nexters.herowars",
        "https://play.google.com/store/apps/details?id=com.panoramik.mightyparty",
        "https://play.google.com/store/apps/details?id=com.ea.game.starwarscapital_row",
        "https://play.google.com/store/apps/details?id=com.ea.games.simsfreeplay_row",
        "https://play.google.com/store/apps/details?id=com.onemt.and.kc",
        "https://play.google.com/store/apps/details?id=com.featherweightgames.fx",
        "https://play.google.com/store/apps/details?id=droidhang.twgame.restaurant",
        "https://play.google.com/store/apps/details?id=com.ninjakiwi.bloonstdbattles",
        "https://play.google.com/store/apps/details?id=com.Gcenter.Raid.Royal.TowerDefense.TD",
        "https://play.google.com/store/apps/details?id=com.ninjakiwi.bloonstdbattles2"
    };

    private static string _password = "Pa$$w0rd";
    public static Dictionary<UserRegister_DTO, UserRole> UsersRolesDict = new()
    {
        { new UserRegister_DTO{Email = "admin-1@google.com", Username = "Oliwia", Password = _password, ConfirmPassword = _password}, UserRole.Admin },
        { new UserRegister_DTO{Email = "admin-2@google.com", Username = "Klara", Password = _password, ConfirmPassword = _password}, UserRole.Admin },
        { new UserRegister_DTO{Email = "admin-3@google.com", Username = "Aurelia", Password = _password, ConfirmPassword = _password}, UserRole.Admin },

        { new UserRegister_DTO{Email = "user-1@google.com", Username = "Hubert", Password = _password, ConfirmPassword = _password}, UserRole.User },
        { new UserRegister_DTO{Email = "user-2@google.com", Username = "Robert", Password = _password, ConfirmPassword = _password}, UserRole.User },
        { new UserRegister_DTO{Email = "user-3@google.com", Username = "Norbert", Password = _password, ConfirmPassword = _password}, UserRole.User },
        { new UserRegister_DTO{Email = "user-4@google.com", Username = "Gilbert", Password = _password, ConfirmPassword = _password}, UserRole.User },
        { new UserRegister_DTO{Email = "user-5@google.com", Username = "Albert", Password = _password, ConfirmPassword = _password}, UserRole.User },
        { new UserRegister_DTO{Email = "user-6@google.com", Username = "Rambert", Password = _password, ConfirmPassword = _password}, UserRole.User },
        { new UserRegister_DTO{Email = "user-7@google.com", Username = "Engelbert", Password = _password, ConfirmPassword = _password}, UserRole.User },
        { new UserRegister_DTO{Email = "user-8@google.com", Username = "Osbert", Password = _password, ConfirmPassword = _password}, UserRole.User }
    };
}
