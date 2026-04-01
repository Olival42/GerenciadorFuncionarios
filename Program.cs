using GerenciadorFuncionarios.Data;
using Microsoft.EntityFrameworkCore;
using GerenciadorFuncionarios.Mappings;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using StackExchange.Redis;
using GerenciadorFuncionarios.Exceptions;
using GerenciadorFuncionarios.Infra;
using GerenciadorFuncionarios.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRateLimiter(options =>
{
    options.RegisterRateLimits();
});

builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add<GlobalErrorHandler>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis");
    return ConnectionMultiplexer.Connect(configuration!);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = TokenValidationConfig.GetTokenValidationParameters(builder.Configuration);
        options.Events = JwtEventsConfig.GetEvents();
    });

builder.Services.AddAuthorization();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
        DataErrorHandler.OnException(context);
});

builder.Services.AddSignalR();

builder.Services
    .AddRepositories()
    .AddServices()
    .AddSecurity()
    .AddCache();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

MapsterConfig.RegisterMappings();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<EstoqueHub>("/estoqueHub");

using (var scope = app.Services.CreateScope())
{
    if (app.Environment.EnvironmentName != "Testing")
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        AppDbInitializer.Seed(db, logger);
    }
}

app.Run();