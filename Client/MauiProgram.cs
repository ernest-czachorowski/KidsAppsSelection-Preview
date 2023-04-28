namespace Client;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder.UseMauiApp<App>().ConfigureFonts(fonts =>
		{
			fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
		});

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddScoped<IAppService, AppService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IResponseManagerService, ResponseManagerService>();
        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.AddSingleton<PasswordVisibilityState>();
        builder.Services.AddSingleton<RefreshTokenState>();

        builder.Services.AddOptions();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
        builder.Services.AddMudServices();

        var baseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5131" : "http://localhost:5131";
		builder.Services.AddScoped(sp => new HttpClient(new CookieHandler()) { BaseAddress = new Uri(baseAddress) });

        return builder.Build();
	}
}

