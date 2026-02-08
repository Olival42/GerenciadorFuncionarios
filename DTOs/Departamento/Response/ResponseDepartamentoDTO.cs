namespace GerenciadorFuncionarios.DTOs.Departamento.Response;

using System;

public record ResponseDepartamentoDTO(Guid Id, string Name, bool IsActive) { }
