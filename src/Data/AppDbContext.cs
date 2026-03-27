namespace GerenciadorFuncionarios.Data;

using Microsoft.EntityFrameworkCore;
using GerenciadorFuncionarios.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Funcionario> Funcionario { get; set; }
    public DbSet<Departamento> Departamento { get; set; }
    public DbSet<Usuario> Usuario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .HasDiscriminator<string>("UserType")
            .HasValue<Funcionario>("Funcionario");
    }
}
