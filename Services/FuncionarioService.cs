namespace GerenciadorFuncionarios.Services;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GerenciadorFuncionarios.Data;
using GerenciadorFuncionarios.DTOs.Funcionario.Requests;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Shared.Responses;
using Mapster;
using GerenciadorFuncionarios.Exceptions;

public class FuncionarioService
{
    private readonly AppDbContext _context;

    public FuncionarioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> RegistrarFuncionarioAsync(RegisterFuncionarioDTO data)
    {
        if (await _context.Funcionario.AnyAsync(f => f.CPF == data.CPF))
            throw new CPFAlreadyExistsException("CPF já cadastrado para outro funcionário.");

        var departamento = await _context.Departamento.FindAsync(data.DepartamentoId);

        if (departamento == null)
            throw new EntityNotFoundException("Departamento não encontrado.");

        var func = data.Adapt<Funcionario>();

        func.Departamento = departamento;

        _context.Funcionario.Add(func);
        await _context.SaveChangesAsync();

        var dto = func.Adapt<ResponseFuncionarioDTO>();

        return ApiResponse<ResponseFuncionarioDTO>.Ok(dto);
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> ObterFuncionarioPorId(Guid id)
    {
        var func = await _context.Funcionario.FindAsync(id);

        if (func == null)
            throw new EntityNotFoundException("Funcionário não encontrado.");

        var dto = func.Adapt<ResponseFuncionarioDTO>();
        return ApiResponse<ResponseFuncionarioDTO>.Ok(dto);
    }
}
