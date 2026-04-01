using Microsoft.EntityFrameworkCore;
using Xunit;
using GerenciadorFuncionarios.Data;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Enums;

namespace GerenciadorFuncionarios.Tests.Data
{
    public class AppDbContextTests
    {
        private DbContextOptions<AppDbContext> GetInMemoryOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        [Fact]
        public void AppDbContext_Should_Instantiate_WithoutErrors()
        {
            var options = GetInMemoryOptions();

            using var context = new AppDbContext(options);
            Assert.NotNull(context);
        }


        [Fact]
        public void AppDbContext_Should_Have_DbSets_NotNull()
        {
            var options = GetInMemoryOptions();

            using var context = new AppDbContext(options);
            Assert.NotNull(context.Funcionario);
            Assert.NotNull(context.Usuario);
        }

        [Fact]
        public void AppDbContext_Should_Set_Discriminator_For_Funcionario()
        {
            var options = GetInMemoryOptions();

            using (var context = new AppDbContext(options))
            {
                Usuario usuario = new Funcionario
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    Phone = "44999999999",
                    CPF = "68714247097",
                    Email = "teste@email.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Role = Role.ADMIN,
                    IsActive = true
                };
                context.Usuario.Add(usuario);
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var usuario = context.Usuario.FirstOrDefault();
                Assert.NotNull(usuario);
                Assert.IsType<Funcionario>(usuario);
            }
        }

    }
}
