using Microsoft.EntityFrameworkCore;
using GerenciadorFuncionarios.Models;

namespace GerenciadorFuncionarios.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Funcionario> Funcionario { get; set; }
    public DbSet<Departamento> Departamento { get; set; }

}
