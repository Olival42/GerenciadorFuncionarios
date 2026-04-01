using Xunit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Logging;
using GerenciadorFuncionarios.Infrastructure;
using GerenciadorFuncionarios.Infrastructure.Seed;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Models;
public class AppDbInitializerTests
{
    [Fact]
    public void Seed_Should_Create_Admin_If_NotExists()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var logger = new Mock<ILogger>().Object;

        AppDbInitializer.Seed(context, logger);

        var admin = context.Funcionario.FirstOrDefault(f => f.Role == Role.GERENTE);
        Assert.NotNull(admin);
        Assert.Equal("Administrador", admin.Name);
    }

    [Fact]
    public void Seed_Should_Not_Create_Admin_If_Exists()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);

        var admin = new Funcionario
        {
            Email = "admin@admin.com",
            Name = "Administrador",
            Phone = "000000000",
            CPF = "00000000000",
            Role = Role.GERENTE,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("GERENTE@123"),
            IsActive = true
        };

        context.Funcionario.Add(admin);
        context.SaveChanges();

        var mockLogger = new Mock<ILogger>();
        AppDbInitializer.Seed(context, mockLogger.Object);

        var admins = context.Funcionario.Where(f => f.Role == Role.GERENTE).ToList();
        Assert.Single(admins);
    }
}