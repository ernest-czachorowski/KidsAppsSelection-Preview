namespace Application.Services.JwtBearerOptionsService;

public class JwtBearerOptionsService : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly IConfiguration _configuration;
    private readonly ITokenBlockingService _tokenBlockingService;

    public JwtBearerOptionsService(IConfiguration configuration, ITokenBlockingService tokenBlockingService)
    {
        _configuration = configuration;
        _tokenBlockingService = tokenBlockingService;
    }

    public void Configure(JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:TokenKey").Value!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents();

        options.Events.OnTokenValidated = async context =>
        {
            if (context.Result is not null && context.Result.Succeeded == false)
                return;

            if (!context.Request.Headers.TryGetValue("Authorization", out var bearer))
            {
                context.Fail("Token is invalid.");
                return;
            }

            var token = bearer.ToString().Split(' ').Length > 1 ? bearer.ToString().Split(' ')[1] : null;

            if (string.IsNullOrEmpty(token))
            {
                context.Fail("Token is invalid.");
                return;
            }

            if (await _tokenBlockingService.IsTokenBlockedOrInvalid(token))
                context.Fail("Token is blocked.");
        };

        options.Events.OnChallenge = context =>
        {
            context.Response.Headers.Remove("Access-Control-Allow-Origin");
            context.Response.Headers.Add("Access-Control-Allow-Origin", "https://localhost:7272");

            context.Response.Headers.Remove("Access-Control-Allow-Credentials");
            context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");

            context.Response.Headers.Remove("Access-Control-Allow-Headers");
            context.Response.Headers.Add("Access-Control-Allow-Headers", "WWW-Authenticate");

            context.Response.Headers.Remove("Access-Control-Expose-Headers");
            context.Response.Headers.Add("Access-Control-Expose-Headers", "WWW-Authenticate");

            return Task.CompletedTask;
        };
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }
}