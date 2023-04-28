namespace Application.Extensions;

public static class ServerServiceExtensions
{
    public static IServiceCollection AddServerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "JWTToken_Auth_API",
                Version = "v1"
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\nEnter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer xyz...\"",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        services.AddCors(o =>
        {
            o.AddPolicy("AllowAll", a => a.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("https://localhost:7272"));
        });

        services.AddDbContext<DataContext>(options =>
        {
            //options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddAutoMapper(typeof(AutoMapper.MappingProfiles).Assembly);

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAppService, AppService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IReceivedTokenService, ReceivedTokenService>();
        services.AddScoped<IDailyRandomService, DailyRandomService>();
        services.AddScoped<IAppFromUrlService, AppFromUrlService>();
        services.AddScoped<SeedDatabaseService>();

        services.AddHostedService<AutorunService>();
        services.AddSingleton<ITokenBlockingService, TokenBlockingService>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer();

        services.ConfigureOptions<JwtBearerOptionsService>();
        services.AddHttpContextAccessor();

        services.AddHttpsRedirection(options =>
        {
            options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
            options.HttpsPort = 443;
        });

        return services;
    }
}

