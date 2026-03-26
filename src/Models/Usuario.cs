namespace GerenciadorFuncionarios.Models;

using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using GerenciadorFuncionarios.Enums;

[Index(nameof(Email), IsUnique = true)]
public abstract class Usuario
{
	[Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public required Role Role { get; set; }
}
