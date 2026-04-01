namespace GerenciadorFuncionarios.Infra;

using GerenciadorFuncionarios.Data;
using GerenciadorFuncionarios.Enums;
using GerenciadorFuncionarios.Models;
using Microsoft.EntityFrameworkCore;

public static class AppDbInitializer
{
    public static void Seed(AppDbContext db, ILogger logger)
    {
        logger.LogInformation("Iniciando seed do banco de dados");

        if (db.Database.IsRelational())
        {
            db.Database.Migrate();
        }

        if (!db.Funcionario.Any(f => f.Role == Role.GERENTE))
        {
            logger.LogInformation("Criando usuário administrador");

            var admin = new Funcionario
            {
                Email = "admin@admin.com",
                Name = "Administrador",
                Phone = "000000000",
                CPF = "00000000000",
                Role = Role.GERENTE,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                IsActive = true
            };

            db.Funcionario.Add(admin);
            db.SaveChanges();

            logger.LogInformation("Usuário administrador criado com sucesso");
        }
        else
        {
            logger.LogInformation("Usuário administrador já existe");
        }
    }
}