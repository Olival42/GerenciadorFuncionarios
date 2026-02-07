using System;

using GerenciadorFuncionarios.Data;
using GerenciadorFuncionarios.DTOs.Departamento.Requests;
using GerenciadorFuncionarios.DTOs.Departamento.Response;

public class DepartamentoService
{
	private readonly AppDbContext _context;

	public DepartamentoService(AppDbContext context)
	{
		_context = context;
	}

	public void RegistrarDepartamento(RegistrarDepartamentoDTO data)
	{
		
	}
}
