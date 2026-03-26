using GerenciadorFuncionarios.Data;
using Microsoft.EntityFrameworkCore;
using GerenciadorFuncionarios.Services;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Mappings;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using GerenciadorFuncionarios.Services.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GerenciadorFuncionarios.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

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
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
        };
    });

builder.Services.AddAuthorization();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value!.Errors.Count() > 0)
            .SelectMany(x => x.Value!.Errors.Select(e =>
                    new DataErrors
                    (
                        Field: x.Key,
                        Message: e.ErrorMessage
                    )
                )
            ).ToList();

        var response = new ApiResponse<List<DataErrors>>(
            Success: false,
            Data: errors,
            Error: null,
            Timestamp: DateTimeOffset.UtcNow
        );

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddScoped<DepartamentoService>();
builder.Services.AddScoped<FuncionarioService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RedisService>();
builder.Services.AddScoped<UserContextService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

MapsterConfig.RegisterMappings();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    db.Database.Migrate();

    if (!db.Funcionario.Any(f => f.Role == GerenciadorFuncionarios.Enums.Role.ADMIN))
    {
        var admin = new Funcionario
        {
            Email = "admin@admin.com",
            Name = "Administrador",
            Phone = "000000000",
            CPF = "00000000000",
            Role = GerenciadorFuncionarios.Enums.Role.ADMIN,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            IsActive = true
        };

        db.Funcionario.Add(admin);
        db.SaveChanges();
    }
}

app.Run();
