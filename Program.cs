using GerenciadorFuncionarios.Data;
using Microsoft.EntityFrameworkCore;
using Mapster;
using GerenciadorFuncionarios.Services;
using GerenciadorFuncionarios.Shared;
using GerenciadorFuncionarios.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Linq;
using System.Collections.Generic;
using GerenciadorFuncionarios.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalErrorHandler>();
});

builder.Services.AddOpenApi();

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

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

MapsterConfig.RegisterMappings();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
