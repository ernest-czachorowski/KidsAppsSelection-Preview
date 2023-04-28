var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Main>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

#if DEBUG
builder.Logging.AddDebug();
builder.Logging.AddFilter("Microsoft.AspNetCore.Authorization.*", LogLevel.None);
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
builder.Services.AddScoped(sp => new HttpClient(new CookieHandler()) { BaseAddress = new Uri("http://localhost:5131") });

await builder.Build().RunAsync();

