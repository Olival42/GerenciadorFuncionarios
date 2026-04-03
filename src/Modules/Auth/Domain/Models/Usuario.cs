namespace GerenciadorFuncionarios.Modules.Auth.Domain.Models;

using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;

[Index(nameof(UserName), IsUnique = true)]
public abstract class Usuario
{
	[Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string UserName { get; set; }

    public required string PasswordHash { get; set; }

    public required Role Role { get; set; }
}
